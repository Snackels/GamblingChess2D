using UnityEngine;

public class Pawn : ChessPieces {
    [SerializeField] private ChessPieceVisual pawnVisual;

    protected new void Awake() {
        chessPieceVisual = pawnVisual;
        base.Awake();
    }
}