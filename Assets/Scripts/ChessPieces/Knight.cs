using UnityEngine;
using System.Collections.Generic;

public class Knight : ChessPieces {

    protected new void Awake() {
        base.Awake();
        baseCost = 25f;
    }

    public override List<Cell> GetThreatenedCells() {
        return GetLShapeCells();
    }

    public override List<Cell> GetValidMoveCells() {
        return GetLShapeCells();
    }

    List<Cell> GetLShapeCells() {
        List<Cell> cells = new List<Cell>();
        if (mCurrentCell == null) return cells;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        // All 8 possible L-shape jumps
        int[,] moves = {
            { 2, 1 }, { 2, -1 },   // right L
            { -2, 1 }, { -2, -1 }, // left L
            { 1, 2 }, { -1, 2 },   // up L
            { 1, -2 }, { -1, -2 }  // down L
        };

        for (int i = 0; i < moves.GetLength(0); i++) {
            Cell cell = mCurrentCell.mBoard.GetCell(
                x + moves[i, 0],
                y + moves[i, 1]
            );
            // Knight jumps over pieces — no blocking check needed
            if (cell != null) cells.Add(cell);
        }

        return cells;
    }
}
