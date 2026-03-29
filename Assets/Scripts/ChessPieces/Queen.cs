using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPieces {

    protected new void Awake() {
        base.Awake();
        baseCost = 90f;
    }

    public override List<Cell> GetThreatenedCells() {
        return GetAllDirectionCells();
    }

    public override List<Cell> GetValidMoveCells() {
        return GetAllDirectionCells();
    }

    List<Cell> GetAllDirectionCells() {
        List<Cell> cells = new List<Cell>();
        if (mCurrentCell == null) return cells;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        // All 8 directions
        int[,] directions = {
            { 1, 0 }, { -1, 0 },  // horizontal
            { 0, 1 }, { 0, -1 },  // vertical
            { 1, 1 }, { -1, 1 },  // diagonal up
            { 1, -1 }, { -1, -1 } // diagonal down
        };

        for (int d = 0; d < directions.GetLength(0); d++) {
            int dx = directions[d, 0];
            int dy = directions[d, 1];

            for (int i = 1; i < 5; i++) {
                Cell cell = mCurrentCell.mBoard.GetCell(x + dx * i, y + dy * i);
                if (cell == null) break;
                if (cell.mCurrentPiece != null) break;
                cells.Add(cell);
            }
        }

        return cells;
    }
}
