using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _containerImage;
    [SerializeField] private Stress _stress;

    [Header("Fill Sprites per Stress Level")]
    [SerializeField] private Sprite fillSprite0;
    [SerializeField] private Sprite fillSprite25;
    [SerializeField] private Sprite fillSprite50;
    [SerializeField] private Sprite fillSprite75;
    [SerializeField] private Sprite fillSprite100;

    [Header("Animation")]
    [SerializeField] private SkeletonGraphic _skeletonGraphic;
    [SpineAnimation(dataField: "skeletonGraphic")]
    public string stressBarAnimationName = "0to100";

    private void Start()
    {
        if (_stress == null || _skeletonGraphic == null)
        {
            Debug.LogError("Missing references.");
            return;
        }

        _stress.OnStressed += UpdateBar;
        _stress.OnStressReduced += UpdateBar;
        _stress.OnMaxStressReached += HandleMaxStress;

        UpdateBar(0);
    }

    private void OnDestroy()
    {
        if (_stress == null) return;

        _stress.OnStressed -= UpdateBar;
        _stress.OnStressReduced -= UpdateBar;
        _stress.OnMaxStressReached -= HandleMaxStress;
    }

    private void UpdateBar(float stress)
    {
        // float normalized = _stress.CurrentStress / _stress.MaxStress;
        // _fillImage.fillAmount = Mathf.Clamp01(normalized);

        float normalized = Mathf.Clamp01(_stress.CurrentStress / _stress.MaxStress);

        SetSpineAnimationToProgress(normalized);

        Sprite selectedSprite = fillSprite0;

        if (normalized >= 1f)
            selectedSprite = fillSprite100;
        else if (normalized >= 0.75f)
            selectedSprite = fillSprite75;
        else if (normalized >= 0.5f)
            selectedSprite = fillSprite50;
        else if (normalized >= 0.25f)
            selectedSprite = fillSprite25;

        _containerImage.sprite = selectedSprite;

        // if (_marker != null && _fillRect != null)
        // {
        //     float width = _fillRect.rect.width;

        //     // Position the marker at the end of the fill
        //     Vector2 newPos = _marker.anchoredPosition;
        //     newPos.x = width * normalized;
        //     _marker.anchoredPosition = newPos;

        //     // Hide if at 0% or 100%
        //     bool shouldHide = normalized <= 0f || normalized >= 1f;
        //     _marker.gameObject.SetActive(!shouldHide);
        // }
    }

    private void HandleMaxStress()
    {
        UpdateBar(0);
        Debug.Log("Игрок перегружен стрессом!");
    }
    
    private void SetSpineAnimationToProgress(float percent)
    {
        var current = _skeletonGraphic.AnimationState.GetCurrent(0);

        if (current == null || current.Animation.Name != stressBarAnimationName)
        {
            _skeletonGraphic.AnimationState.SetAnimation(0, stressBarAnimationName, false);
            current = _skeletonGraphic.AnimationState.GetCurrent(0);
        }

        _skeletonGraphic.AnimationState.TimeScale = 0;

        float duration = current.Animation.Duration;
        current.TrackTime = duration * percent;

        _skeletonGraphic.AnimationState.Apply(_skeletonGraphic.Skeleton);
        _skeletonGraphic.LateUpdate(); // Make sure the skeleton updates in UI
    }
}