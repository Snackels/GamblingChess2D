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
}