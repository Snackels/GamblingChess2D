using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject visualPrefab;
    [SerializeField] Transform visualParent;

    ChessPieces activePiece = null;
    ChessPieces waitingPiece = null;

    public void SpawnPiece() {
        if (piecePrefab == null) return;
        if (waitingPiece != null) return;

        int spawnTurn = TurnManager.Instance.currentTurn;
        Debug.Log($"SpawnPiece called, spawnTurn={spawnTurn}");

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
        capturedWaiting.OnPiecePlaced = (placedPiece) => OnWaitingPiecePlaced(capturedWaiting, spawnTurn);
        capturedWaiting.OnPieceSold = (soldPiece) => OnPieceSold(soldPiece, spawnTurn);
    }

    void OnWaitingPiecePlaced(ChessPieces piece, int spawnTurn) {
        if (piece != waitingPiece) return;
        activePiece = piece;
        waitingPiece = null;
        piece.OnPiecePlaced = null;
        Debug.Log($"Piece placed, spawnTurn={spawnTurn}");
    }

    void OnPieceSold(ChessPieces soldPiece, int spawnTurn) {
        bool wasWaiting = soldPiece == waitingPiece;
        bool wasActive = soldPiece == activePiece;

        if (wasWaiting) waitingPiece = null;
        if (wasActive) activePiece = null;

        Debug.Log($"Piece sold. wasWaiting={wasWaiting}, spawnTurn={spawnTurn}, currentTurn={TurnManager.Instance.currentTurn}");

        if (wasWaiting) {
            SpawnPiece();
            return;
        }

        if (wasActive && spawnTurn == TurnManager.Instance.currentTurn) {
            SpawnPiece();
        }
    }
}