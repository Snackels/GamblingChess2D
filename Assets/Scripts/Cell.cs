using UnityEngine;

public class Cell : MonoBehaviour {
    [HideInInspector] public Vector2Int mBoardPosition = Vector2Int.zero;
    [HideInInspector] public Board mBoard = null;
    [HideInInspector] public RectTransform mRectTransform = null;

    public void Setup(Vector2Int newBoardPosition, Board newBoard) {
        mBoardPosition = newBoardPosition;
        mBoard = newBoard;
        mRectTransform = GetComponent<RectTransform>();
    }
}
