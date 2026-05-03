using UnityEngine;
[ExecuteAlways]
public class ContentPlacer : MonoBehaviour
{
    [SerializeField] bool listLike;
    [SerializeField] bool excludeInactive;
    [SerializeField] Vector2 startOffset;
    [SerializeField] protected float rowSpacing;
    [SerializeField] protected int colCount;
    [SerializeField] protected float colSpacing;

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
    public void RecalculatePositions()
    {
        int index = 0;
        int colIndex = 0;
        Vector2 prevPos = new(startOffset.x, startOffset.y + rowSpacing);
        float totalHeight = 0;

        foreach (Transform child in transform)
        {
            if (excludeInactive && !child.gameObject.activeInHierarchy)
                continue;

            if (colCount-1 > 0 && index > 0 && colIndex < colCount-1 && index % colCount-1 >= 0)
            {
                child.localPosition = new Vector3(prevPos.x + colSpacing, prevPos.y, 0);
                colIndex++;
            }
            else
            {
                var childRect = child.GetComponent<RectTransform>();
                totalHeight += rowSpacing + childRect.sizeDelta.y;

                child.localPosition = new Vector3(startOffset.x, prevPos.y - rowSpacing, 0);
                colIndex = 0;
            }
            prevPos = child.localPosition;
            index++;
        }
        if (listLike)
        {
            totalHeight += rowSpacing;
            var rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, totalHeight/2f);
        }
    }
}