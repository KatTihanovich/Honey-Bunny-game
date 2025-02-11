using UnityEngine;

public class SyncUIPosition : MonoBehaviour
{
    public RectTransform sourceElement;  // UI element on the first canvas
    public RectTransform targetElement;  // UI element on the second canvas (inside the panel)
    public Canvas secondCanvas; 

    private void Update()
    {
        if (sourceElement == null || targetElement == null || secondCanvas == null)
            return;

        Vector3 worldPos = sourceElement.position;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            secondCanvas.transform as RectTransform,
            screenPos,
            secondCanvas.worldCamera,
            out localPos
        );

        targetElement.localPosition = localPos;
    }
}
