using UnityEngine;

public class Bishop : ChessPieces {
    [SerializeField] private ChessPieceVisual bishopVisual;

    protected new void Awake() {
        chessPieceVisual = bishopVisual;
        base.Awake();
    }
}
