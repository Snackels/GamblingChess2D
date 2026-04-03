using UnityEngine;

public class Rook : ChessPieces {
    [SerializeField] private ChessPieceVisual rookVisual;

    protected new void Awake() {
        chessPieceVisual = rookVisual;
        base.Awake();
    }
}
