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
        if (leftStar != null)
        {
            leftStar.gameObject.SetActive(false);
            leftStar.anchoredPosition = Vector2.zero;
        }

        if (rightStar != null)
        {
            rightStar.gameObject.SetActive(false);
            rightStar.anchoredPosition = Vector2.zero;
        }
    }

    void Update()
    {
        if (leftStar != null)
            leftStar.anchoredPosition = Vector2.Lerp(leftStar.anchoredPosition, leftTargetPos, Time.unscaledDeltaTime * moveSpeed);

        if (rightStar != null)
            rightStar.anchoredPosition = Vector2.Lerp(rightStar.anchoredPosition, rightTargetPos, Time.unscaledDeltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        if (leftStar != null)
        {
            leftStar.gameObject.SetActive(true);
            leftTargetPos = new Vector2(-hoverOffset, 0);
        }

        if (rightStar != null)
        {
            rightStar.gameObject.SetActive(true);
            rightTargetPos = new Vector2(hoverOffset, 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        StartCoroutine(HideStarsAfter());
    }

    private System.Collections.IEnumerator HideStarsAfter()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if (!hovered)
        {
            if (leftStar != null)
                leftStar.gameObject.SetActive(false);

            if (rightStar != null)
                rightStar.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hovered) return;

        if (leftStar != null)
            leftTargetPos = new Vector2(-hoverOffset + clickMoveAmount, 0);

        if (rightStar != null)
            rightTargetPos = new Vector2(hoverOffset - clickMoveAmount, 0);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(HideStarsAfter());
    }
}
