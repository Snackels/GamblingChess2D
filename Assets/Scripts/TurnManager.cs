using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance;
    public int currentTurn = 0;

    void Awake() {
        Instance = this;
    }

    public void EndTurn() {
        StartCoroutine(EndTurnRoutine());
    }

    IEnumerator EndTurnRoutine() {
        yield return StartCoroutine(ScoreManager.Instance.CalculateScoreCoroutine(GameManager.Instance.mBoard));

        ChessPieces[] allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);

        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell == null) continue;

            if (piece.isSelectedForUpkeep) {
                float upkeep = piece.GetUpkeepCost();
                if (ScoreManager.Instance.CanAfford(upkeep)) {
                    ScoreManager.Instance.SpendMoney(upkeep);
                    if (!piece.isActive)
                        piece.SetActive();
                }
                else {
                    piece.SetInactive();
                }
            }
            else {
                piece.SetInactive();
            }

            piece.turnsOnBoard++;

            piece.ClearUpkeepSelection();
        }

        ThreatManager.Instance.RecalculateAllThreats();

        allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);
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
            if (piece.mCurrentCell != null && piece.isActive)
                piece.SetMustMove(true);
        }
    }
}