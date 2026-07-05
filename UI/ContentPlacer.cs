using UnityEngine;

[ExecuteAlways]
public class ContentPlacer : MonoBehaviour
{
    [SerializeField] bool listLike;
    [SerializeField] bool excludeInactive;
    [SerializeField] bool autoCenter;
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
        int cols = Mathf.Max(colCount, 1);

        // Count children that will actually be laid out.
        int visibleCount = 0;
        foreach (Transform child in transform)
        {
            if (excludeInactive && !child.gameObject.activeSelf) continue;
            visibleCount++;
        }

        int rowCount = visibleCount > 0 ? Mathf.CeilToInt(visibleCount / (float)cols) : 0;

        if (autoCenter)
        {
            startOffset.x = -((cols - 1) * colSpacing) / 2f;
            startOffset.y = ((rowCount - 1) * rowSpacing) / 2f;
        }

        int colIndex = 0;
        int rowIndex = 0;
        float lastRowHeight = 0f;

        foreach (Transform child in transform)
        {
            if (excludeInactive && !child.gameObject.activeSelf)
                continue;

            float x = startOffset.x + colIndex * colSpacing;
            float y = startOffset.y - rowIndex * rowSpacing;
            child.localPosition = new Vector3(x, y, 0f);

            // Track the tallest item in the final row so the fitter below
            // can size to the actual content instead of just another "pitch".
            if (rowIndex == rowCount - 1)
            {
                var childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                    lastRowHeight = Mathf.Max(lastRowHeight, childRect.sizeDelta.y);
            }

            colIndex++;
            if (colIndex >= cols)
            {
                colIndex = 0;
                rowIndex++;
            }
        }

        if (listLike)
        {
            var rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                float totalHeight = rowCount > 0
                    ? (rowCount - 1) * rowSpacing + lastRowHeight
                    : 0f;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, totalHeight);
            }
        }
    }
}