using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
    public Image mOutlineImage;
    public Image mThreatImage;
    public TextMeshProUGUI mThreatText;

    [HideInInspector] public Vector2Int mBoardPosition = Vector2Int.zero;
    [HideInInspector] public Board mBoard = null;
    [HideInInspector] public RectTransform mRectTransform = null;
    [HideInInspector] public ChessPieces mCurrentPiece = null;
    [HideInInspector] public int mThreatCount = 0;
    [HideInInspector] public bool isHighlighted = false;

    Canvas overlayCanvas;

    public void Setup(Vector2Int newBoardPosition, Board newBoard, Canvas overlay) {
        mBoardPosition = newBoardPosition;
        mBoard = newBoard;
        mRectTransform = GetComponent<RectTransform>();
        overlayCanvas = overlay;

        mThreatText.transform.SetParent(overlayCanvas.transform, false);
        UpdateThreatDisplay();
    }

    void Update() {
        if (mThreatText != null && mThreatCount > 0) {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                Camera.main, transform.position
            );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlayCanvas.transform as RectTransform,
                screenPos,
                null,
                out Vector2 localPos
            );
            mThreatText.rectTransform.localPosition = localPos;
        }
    }

    public void AddThreat() {
        mThreatCount++;
        UpdateThreatDisplay();
    }

    public void RemoveThreat() {
        mThreatCount = Mathf.Max(0, mThreatCount - 1);
        UpdateThreatDisplay();
    }

    public void ClearThreat() {
        mThreatCount = 0;
        UpdateThreatDisplay();
    }

    void UpdateThreatDisplay() {
        bool isThreatened = mThreatCount > 0;
        mThreatImage.enabled = isThreatened;
        mThreatText.enabled = isThreatened;
        if (isThreatened) {
            mThreatText.text = mThreatCount.ToString();
            mThreatImage.color = new Color(1, 0, 0, Mathf.Clamp(0.2f * mThreatCount, 0.2f, 0.8f));
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null) return;
        ChessPieces piece = eventData.pointerDrag.GetComponent<ChessPieces>();
        if (piece == null) return;
        if (!piece.isDragging) return;
        piece.mTargetCell = this;
        mOutlineImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag == null) return;
        ChessPieces piece = eventData.pointerDrag.GetComponent<ChessPieces>();
        if (piece == null) return;
        if (!piece.isDragging) return;
        if (piece.mTargetCell == this)
            piece.mTargetCell = null;
        mOutlineImage.enabled = isHighlighted;
    }

    public void OnDrop(PointerEventData eventData) {
        ChessPieces piece = eventData.pointerDrag?.GetComponent<ChessPieces>();
        if (piece == null) return;
        mOutlineImage.enabled = isHighlighted;
    }

    void OnDestroy() {
        if (mThreatText != null)
            Destroy(mThreatText.gameObject);
    }
}