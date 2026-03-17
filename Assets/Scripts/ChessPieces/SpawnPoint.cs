using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject visualPrefab;
    [SerializeField] Transform visualParent;

    ChessPieces activePiece = null;
    ChessPieces waitingPiece = null;

    public void SpawnPiece() {
        if (piecePrefab == null) return;
        if (waitingPiece != null && waitingPiece.gameObject != null) return;
        waitingPiece = null;

        GameObject newPiece = Instantiate(piecePrefab, transform.parent);
        newPiece.GetComponent<RectTransform>().anchoredPosition =
            GetComponent<RectTransform>().anchoredPosition;

        ChessPieces piece = newPiece.GetComponent<ChessPieces>();

        if (visualPrefab != null) {
            GameObject newVisual = Instantiate(visualPrefab, visualParent);
            ChessPieceVisual visual = newVisual.GetComponent<ChessPieceVisual>();
            piece.chessPieceVisual = visual;
            visual.Initialize(piece);
        }

        waitingPiece = piece;

        ChessPieces capturedWaiting = piece;
        capturedWaiting.OnPiecePlaced = (placedPiece) => OnWaitingPiecePlaced(capturedWaiting);
        capturedWaiting.OnPieceSold = (soldPiece) => OnPieceSold(soldPiece);
    }

    void OnWaitingPiecePlaced(ChessPieces piece) {
        if (piece != waitingPiece) return;
        activePiece = piece;
        waitingPiece = null;
        piece.OnPiecePlaced = null;
    }

    void OnPieceSold(ChessPieces soldPiece) {
        bool wasWaiting = soldPiece == waitingPiece;
        bool wasActive = soldPiece == activePiece;

        if (wasWaiting) waitingPiece = null;
        if (wasActive) activePiece = null;

        int spawnTurn = soldPiece != null ? soldPiece.spawnTurn : -1;

        if (wasWaiting) {
            StartCoroutine(SpawnNextFrame());
            return;
        }

        if (wasActive && spawnTurn == TurnManager.Instance.currentTurn)
            StartCoroutine(SpawnNextFrame());
    }

    IEnumerator SpawnNextFrame() {
        yield return null;
        SpawnPiece();
    }
}