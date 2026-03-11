using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
    public Image mOutlineImage;
    [HideInInspector] public Vector2Int mBoardPosition = Vector2Int.zero;
    [HideInInspector] public Board mBoard = null;
    [HideInInspector] public RectTransform mRectTransform = null;
    [HideInInspector] public ChessPieces mCurrentPiece = null;

    public void Setup(Vector2Int newBoardPosition, Board newBoard) {
        mBoardPosition = newBoardPosition;
        mBoard = newBoard;
        mRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null) return;
        ChessPieces piece = eventData.pointerDrag.GetComponent<ChessPieces>();
        if (piece == null) return;

        piece.mTargetCell = this;
        mOutlineImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag == null) return;
        ChessPieces piece = eventData.pointerDrag.GetComponent<ChessPieces>();
        if (piece == null) return;

        if (piece.mTargetCell == this)
            piece.mTargetCell = null;

        mOutlineImage.enabled = false;
    }

    public void OnDrop(PointerEventData eventData) {
        ChessPieces piece = eventData.pointerDrag?.GetComponent<ChessPieces>();
        if (piece == null) return;
        mOutlineImage.enabled = false;
    }
}