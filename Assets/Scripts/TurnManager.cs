using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
                    piece.owedUpkeep = 0f;
                    if (!piece.isActive) piece.SetActive();
                }
                else {
                    piece.owedUpkeep = upkeep;
                    piece.SetInactive();
                }
            }
            else {
                piece.owedUpkeep = piece.GetUpkeepCost();
                piece.SetInactive();
            }

            piece.turnsOnBoard++;
            piece.ClearUpkeepSelection();
        }

        ThreatManager.Instance.RecalculateAllThreats();

        TaxManager.Instance.ChargeTax();

        currentTurn++;

        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell == null) continue;
            if (piece.mustMove && !piece.hasMovedThisTurn)
                piece.Penalty();
        }

        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (SpawnPoint sp in spawnPoints)
            sp.SpawnPiece();

        allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);
        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell == null) continue;
            piece.SetMustMove(true);
        }
    }
}