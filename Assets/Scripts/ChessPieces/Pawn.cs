using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPieces {

    protected override void Awake() {
        base.Awake();
        baseCost = 10f;
    }

    public override bool IsValidPlacement(Cell cell) {
        if (mustMove || _wasReactivatedThisTurn) return true;
        return cell.mBoardPosition.y == 0;
    }

    public override List<Cell> GetThreatenedCells() {
        List<Cell> threatened = new List<Cell>();
        if (mCurrentCell == null) return threatened;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        Cell upperLeft = mCurrentCell.mBoard.GetCell(x - 1, y + 1);
        if (upperLeft != null && upperLeft.mCurrentPiece == null)
            threatened.Add(upperLeft);

        Cell upperRight = mCurrentCell.mBoard.GetCell(x + 1, y + 1);
        if (upperRight != null && upperRight.mCurrentPiece == null)
            threatened.Add(upperRight);

        return threatened;
    }

    public override List<Cell> GetValidMoveCells() {
        List<Cell> valid = new List<Cell>();
        if (mCurrentCell == null) return valid;

        int x = mCurrentCell.mBoardPosition.x;
        int y = mCurrentCell.mBoardPosition.y;

        Cell forward = mCurrentCell.mBoard.GetCell(x, y + 1);
        if (forward != null && forward.mCurrentPiece == null)
            valid.Add(forward);

        return valid;
    }
}