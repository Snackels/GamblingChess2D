using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class PanelButton {
    public RectTransform buttonRect;
    public Vector2 offset = new Vector2(0f, 40f);
}

public class CoinPanel : MonoBehaviour {
    [Header("References")]
    public RectTransform panelRect;
    [Tooltip("Add as many buttons as you want — each follows the panel with its own offset")]
    public List<PanelButton> buttons = new List<PanelButton>();
    [Tooltip("Optional — toggles boundary when panel opens/closes")]
    public CoinBoundary coinBoundary;
    [Header("Slide Settings")]
    public float hiddenY = -815f;
    public float shownY = -10f;
    public float slideDuration = 0.45f;
    public Ease slideUpEase = Ease.OutBack;
    public Ease slideDownEase = Ease.InBack;
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
        foreach (var btn in buttons) {
            if (btn.buttonRect != null)
                btn.buttonRect.anchoredPosition = panelRect.anchoredPosition + btn.offset;
        }
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