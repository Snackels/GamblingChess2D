using UnityEngine;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance;
    public int currentTurn = 0;

    void Awake() {
        Instance = this;
    }

    public void EndTurn() {
        ChessPieces[] allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);
        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell != null && piece.mustMove)
                piece.Penalty();
        }

        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (SpawnPoint sp in spawnPoints)
            sp.SpawnPiece();

        currentTurn++;

        allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);
        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell != null)
                piece.SetMustMove(true);
        }
    }
}