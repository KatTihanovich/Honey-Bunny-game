using UnityEngine;
using UnityEngine.EventSystems;

public class StarHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public RectTransform leftStar;
    public RectTransform rightStar;

    public float hoverOffset = 80;     
    public float clickMoveAmount = 30;  
    public float moveSpeed = 20;

    private Vector2 leftTargetPos;
    private Vector2 rightTargetPos;

    private bool hovered = false;

    void Start()
    {
        leftStar.gameObject.SetActive(false);
        rightStar.gameObject.SetActive(false);

        // start centered
        leftStar.anchoredPosition = Vector2.zero;
        rightStar.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        leftStar.anchoredPosition = Vector2.Lerp(leftStar.anchoredPosition, leftTargetPos, Time.deltaTime * moveSpeed);
        rightStar.anchoredPosition = Vector2.Lerp(rightStar.anchoredPosition, rightTargetPos, Time.deltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        leftStar.gameObject.SetActive(true);
        rightStar.gameObject.SetActive(true);

        // perfectly symmetrical
        leftTargetPos = new Vector2(-hoverOffset, 0);
        rightTargetPos = new Vector2(hoverOffset, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        // smoothly disappear to center instead of instantly hiding
        // leftTargetPos = Vector2.zero;
        // rightTargetPos = Vector2.zero;

        // delay hiding so movement is visible
        StartCoroutine(HideStarsAfter());
    }

    private System.Collections.IEnumerator HideStarsAfter()
    {
        yield return new WaitForSeconds(0.2f);
        if (!hovered)
        {
            leftStar.gameObject.SetActive(false);
            rightStar.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // only click while inside button
        if (!hovered) return;

        // move closer**
        leftTargetPos += new Vector2(clickMoveAmount, 0);
        rightTargetPos += new Vector2(-clickMoveAmount, 0);
    }
}
