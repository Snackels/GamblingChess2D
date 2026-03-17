using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPieces {

    protected new void Awake() {
        base.Awake();
    }

    public override List<Cell> GetThreatenedCells() {
        List<Cell> threatened = new List<Cell>();
        if (mCurrentCell == null) return threatened;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        // all 8 directions
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
                if (cell == null) break; // hit board edge, stop
                threatened.Add(cell);
                if (cell.mCurrentPiece != null) break; // blocked by piece, stop
            }
        }

        return threatened;
    }
}
