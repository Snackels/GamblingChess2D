using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPieces {
    [SerializeField] private ChessPieceVisual rookVisual;

    protected override void Awake() {
        chessPieceVisual = rookVisual;
        base.Awake();
    }

    public override List<Cell> GetThreatenedCells() {
        List<Cell> threatened = new List<Cell>();
        if (mCurrentCell == null) return threatened;

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
                if (cell == null) break;        // hit board edge
                threatened.Add(cell);
                if (cell.mCurrentPiece != null) break; // blocked by piece, stop this direction
            }
        }

        return threatened;
    }
}