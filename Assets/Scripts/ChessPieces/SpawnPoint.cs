using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private Transform visualParent;

    // ✅ track the active piece this spawn point owns
    private ChessPieces activePiece = null;

    public void SpawnPiece() {
        if (piecePrefab == null) return;
        // ✅ don't spawn if piece already exists
        if (activePiece != null) return;

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

        // ✅ register piece with this spawn point
        activePiece = piece;
        activePiece.OnPieceSold = OnPieceSold;
    }

    // ✅ called when piece is sold
    // TODO: add turn check here when turn system is implemented
    // e.g. if (TurnManager.Instance.WasPlacedThisTurn(activePiece)) RespawnPiece();

    private void OnPieceSold() {
        activePiece = null;

        // TODO: plug turn check in here
        // if (TurnManager.Instance.WasPlacedThisTurn(activePiece))
        //     RespawnPiece();
        // else
        //     return; // no respawn, piece is gone permanently

        RespawnPiece(); // for now always respawn
    }

    private void RespawnPiece() {
        SpawnPiece();
    }
}