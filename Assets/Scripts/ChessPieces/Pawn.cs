using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPieces {

    protected override void Awake() {
        base.Awake();
    }

    public override List<Cell> GetThreatenedCells() {
        List<Cell> threatened = new List<Cell>();
        if (mCurrentCell == null) return threatened;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        Cell upperLeft = mCurrentCell.mBoard.GetCell(x - 1, y + 1);
        if (upperLeft != null) threatened.Add(upperLeft);

        Cell upperRight = mCurrentCell.mBoard.GetCell(x + 1, y + 1);
        if (upperRight != null) threatened.Add(upperRight);

        return threatened;
    }
}