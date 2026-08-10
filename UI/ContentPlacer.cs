using UnityEngine;

[ExecuteAlways]
public class ContentPlacer : MonoBehaviour
{
    [SerializeField] bool listLike;
    [SerializeField] bool excludeInactive;
    [SerializeField] bool autoCenter;
    [SerializeField] bool swapVerticalAndHorizontal;
    [SerializeField] Vector2 startOffset;
    [SerializeField] protected float rowSpacing;
    [SerializeField] protected int colCount;
    [SerializeField] protected float colSpacing;

    bool shouldRecalculate = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        RecalculatePositions();
    }
#endif

    private void OnTransformChildrenChanged()
    {
        RecalculatePositions();
    }

    private void OnEnable()
    {
        if (!shouldRecalculate) return;
        RecalculatePositions(true);
    }

    public void RecalculatePositions(bool force = false)
    {
        if (!gameObject.activeInHierarchy && !force)
        {
            shouldRecalculate = true;
            return;
        }
        shouldRecalculate = false;

        // Guard against 0/negative column counts (e.g. an un-set inspector field).
        // "cols" is the number of items along the primary fill axis:
        // normally that's horizontal (X), but when swapped it's vertical (Y).
        int cols = Mathf.Max(colCount, 1);

        // Count children that will actually be laid out.
        int visibleCount = 0;
        foreach (Transform child in transform)
        {
            if (excludeInactive && !child.gameObject.activeSelf) continue;
            visibleCount++;
        }

        // "lineCount" is how many wraps are needed along the secondary axis:
        // rows when not swapped, columns when swapped.
        int lineCount = visibleCount > 0 ? Mathf.CeilToInt(visibleCount / (float)cols) : 0;

        if (autoCenter)
        {
            if (!swapVerticalAndHorizontal)
            {
                startOffset.x = -((cols - 1) * colSpacing) / 2f;
                startOffset.y = ((lineCount - 1) * rowSpacing) / 2f;
            }
            else
            {
                startOffset.x = -((lineCount - 1) * colSpacing) / 2f;
                startOffset.y = ((cols - 1) * rowSpacing) / 2f;
            }
        }

        int primaryIndex = 0;
        int lineIndex = 0;
        float lastLineSize = 0f;

        foreach (Transform child in transform)
        {
            if (excludeInactive && !child.gameObject.activeSelf)
                continue;

            float x, y;
            if (!swapVerticalAndHorizontal)
            {
                // Fill horizontally first (primary = X, cols items per row),
                // then wrap down to the next row (line = Y).
                x = startOffset.x + primaryIndex * colSpacing;
                y = startOffset.y - lineIndex * rowSpacing;
            }
            else
            {
                // Fill vertically first (primary = Y, cols items per column),
                // then wrap sideways to the next column (line = X).
                x = startOffset.x + lineIndex * colSpacing;
                y = startOffset.y - primaryIndex * rowSpacing;
            }
            child.localPosition = new Vector3(x, y, 0f);

            // Track the tallest/widest item in the final line so the fitter below
            // can size to the actual content instead of just another "pitch".
            if (lineIndex == lineCount - 1)
            {
                var childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    float size = swapVerticalAndHorizontal ? childRect.sizeDelta.x : childRect.sizeDelta.y;
                    lastLineSize = Mathf.Max(lastLineSize, size);
                }
            }

            primaryIndex++;
            if (primaryIndex >= cols)
            {
                primaryIndex = 0;
                lineIndex++;
            }
        }

        if (listLike)
        {
            var rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                float lineSpacing = swapVerticalAndHorizontal ? colSpacing : rowSpacing;
                float totalSize = lineCount > 0
                    ? (lineCount - 1) * lineSpacing + lastLineSize
                    : 0f;

                rect.sizeDelta = swapVerticalAndHorizontal
                    ? new Vector2(totalSize, rect.sizeDelta.y)
                    : new Vector2(rect.sizeDelta.x, totalSize);
            }
        }
    }
}