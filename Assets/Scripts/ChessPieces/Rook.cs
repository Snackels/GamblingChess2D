using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPieces {

    protected override void Awake() {
        base.Awake();
    }

    public override List<Cell> GetThreatenedCells() {
        return GetRayCells();
    }

    // Rook moves along the same rays — valid targets are all cells in range
    public override List<Cell> GetValidMoveCells() {
        return GetRayCells();
    }

    List<Cell> GetRayCells() {
        List<Cell> cells = new List<Cell>();
        if (mCurrentCell == null) return cells;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        int[,] directions = {
            { 1, 0 }, { -1, 0 },  // horizontal
            { 0, 1 }, { 0, -1 }   // vertical
        };

        for (int d = 0; d < directions.GetLength(0); d++) {
            int dx = directions[d, 0];
            int dy = directions[d, 1];

            for (int i = 1; i < 5; i++) {
                Cell cell = mCurrentCell.mBoard.GetCell(x + dx * i, y + dy * i);
                if (cell == null) break;
                cells.Add(cell);
                if (cell.mCurrentPiece != null) break; // blocked — include the cell but stop the ray
            }
        }

        return cells;
    }
}