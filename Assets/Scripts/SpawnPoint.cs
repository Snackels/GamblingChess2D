using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    [SerializeField] private GameObject piecePrefab;      // UI piece prefab
    [SerializeField] private GameObject visualPrefab;     // 3D root prefab e.g. Pawn3DRoot

    [SerializeField] private Transform visualParent;      // drag your scene root here e.g. a "Pieces3D" empty GameObject

    public void SpawnPiece() {
        if (piecePrefab == null) return;

        // spawn UI piece
        GameObject newPiece = Instantiate(piecePrefab, transform.parent);
        newPiece.GetComponent<RectTransform>().anchoredPosition =
            GetComponent<RectTransform>().anchoredPosition;

        // spawn 3D visual outside canvas
        if (visualPrefab != null) {
            GameObject newVisual = Instantiate(visualPrefab, visualParent);
            ChessPieceVisual visual = newVisual.GetComponent<ChessPieceVisual>();

            // link them together
            ChessPieces piece = newPiece.GetComponent<ChessPieces>();
            piece.chessPieceVisual = visual;
            visual.Initialize(piece);
        }
    }
}