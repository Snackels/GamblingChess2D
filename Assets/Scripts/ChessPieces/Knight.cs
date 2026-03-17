using UnityEngine;
using System.Collections.Generic;

public class Knight : ChessPieces {

    protected new void Awake() {
        base.Awake();
    }

    public override List<Cell> GetThreatenedCells() {
        List<Cell> threatened = new List<Cell>();
        if (mCurrentCell == null) return threatened;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        // all 8 possible L shape jumps
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
            // knight is never blocked so no piece check needed
            if (cell != null) threatened.Add(cell);
        }

        return threatened;
    }
}
