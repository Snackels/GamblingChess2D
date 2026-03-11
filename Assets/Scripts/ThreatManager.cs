using UnityEngine;
using System.Collections.Generic;

public class ThreatManager : MonoBehaviour {
    public static ThreatManager Instance;

    void Awake() {
        Instance = this;
    }

    public void RegisterThreats(ChessPieces piece, List<Cell> threatenedCells) {
        foreach (Cell cell in threatenedCells)
            cell.AddThreat();
    }

    public void UnregisterThreats(ChessPieces piece, List<Cell> threatenedCells) {
        foreach (Cell cell in threatenedCells)
            cell.RemoveThreat();
    }

    public void RecalculateAllThreats() {
        ChessPieces[] allPieces = FindObjectsByType<ChessPieces>(FindObjectsSortMode.None);

        foreach (ChessPieces piece in allPieces)
            if (piece.mCurrentCell != null)
                UnregisterThreats(piece, piece.GetCurrentThreats());

        foreach (ChessPieces piece in allPieces) {
            if (piece.mCurrentCell != null) {
                List<Cell> newThreats = piece.GetThreatenedCells();
                piece.SetCurrentThreats(newThreats);
                RegisterThreats(piece, newThreats);
            }
        }
    }
}