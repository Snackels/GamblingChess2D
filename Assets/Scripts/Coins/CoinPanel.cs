using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CoinPanel : MonoBehaviour {
    [Header("References")]
    public RectTransform panelRect;
    public RectTransform buttonRect;


    [Tooltip("Optional — toggles boundary when panel opens/closes")]
    public CoinBoundary coinBoundary;

    [Header("Slide Settings")]
    public float hiddenY = -815f;
    public float shownY = -10f;
    public float slideDuration = 0.45f;
    public Ease slideUpEase = Ease.OutBack;
    public Ease slideDownEase = Ease.InBack;

    [Header("Button Offset from Panel")]
    [Tooltip("Offset in pixels from the panel's anchoredPosition. " +
             "X=0 keeps it horizontally centered with the panel. " +
             "Y=40 puts it 40px above the panel's anchor point.")]
    public Vector2 buttonOffset = new Vector2(0f, 40f);

    [Header("Hover Scale")]
    [Tooltip("How much to scale UP on hover, added on top of the panel's original scale. 0.04 = 4% bigger.")]
    public float hoverScaleAdd = 0.04f;
    public float scaleDuration = 0.2f;
    public Ease scaleEase = Ease.OutQuad;

    private bool _isOpen = false;
    private Vector3 _originalScale;

    private void Awake() {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        _originalScale = panelRect.localScale;

        Vector2 pos = panelRect.anchoredPosition;
        pos.y = hiddenY;
        panelRect.anchoredPosition = pos;
    }

    private void LateUpdate() {
        if (buttonRect != null)
            buttonRect.anchoredPosition = panelRect.anchoredPosition + buttonOffset;
    }

    public void OnClick() {
        _isOpen = !_isOpen;

        float target = _isOpen ? shownY : hiddenY;
        Ease ease = _isOpen ? slideUpEase : slideDownEase;

        panelRect.DOAnchorPosY(target, slideDuration)
                 .SetEase(ease)
                 .SetUpdate(true);

        coinBoundary?.SetOpen(_isOpen);
    }

    public void OnHoverEnter() {
        Vector3 target = _isOpen
            ? _originalScale * (1f - hoverScaleAdd)
            : _originalScale * (1f + hoverScaleAdd);
        panelRect.DOScale(target, scaleDuration).SetEase(scaleEase);
    }

    public void OnHoverExit() {
        panelRect.DOScale(_originalScale, scaleDuration).SetEase(scaleEase);
    }
}
