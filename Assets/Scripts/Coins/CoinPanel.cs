using UnityEngine;
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

    public List<PanelButton> buttons = new List<PanelButton>();

    public CoinBoundary coinBoundary;
    public CoinSpawner coinSpawner;

    [Header("Slide Settings")]
    public float hiddenY = -815f;
    public float shownY = -10f;
    public float slideDuration = 0.45f;
    public Ease slideUpEase = Ease.OutBack;
    public Ease slideDownEase = Ease.InBack;

    [Header("Hover Scale")]
    public float hoverScaleAdd = 0.04f;
    public float scaleDuration = 0.2f;
    public Ease scaleEase = Ease.OutQuad;
    bool _isOpen = false;
    Vector3 _originalScale;

    void Awake() {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();
        _originalScale = panelRect.localScale;
        Vector2 pos = panelRect.anchoredPosition;
        pos.y = hiddenY;
        panelRect.anchoredPosition = pos;
    }

    void LateUpdate() {
        foreach (var btn in buttons) {
            if (btn.buttonRect != null)
                btn.buttonRect.anchoredPosition = panelRect.anchoredPosition + btn.offset;
        }
    }

    public void OnClick() {
        _isOpen = !_isOpen;
        float target = _isOpen ? shownY : hiddenY;
        Ease ease = _isOpen ? slideUpEase : slideDownEase;
        panelRect.DOAnchorPosY(target, slideDuration).SetEase(ease).SetUpdate(true);
        coinBoundary?.SetOpen(_isOpen);
        if (!_isOpen) coinSpawner?.ClearCoins();
    }

    public void OnHoverEnter() {
        Vector3 target = _isOpen ? _originalScale * (1f - hoverScaleAdd) : _originalScale * (1f + hoverScaleAdd); panelRect.DOScale(target, scaleDuration).SetEase(scaleEase);
    }

    public void OnHoverExit() {
        panelRect.DOScale(_originalScale, scaleDuration).SetEase(scaleEase);
    }
}