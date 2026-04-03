using UnityEngine;

public class Queen : ChessPieces {
    [SerializeField] private ChessPieceVisual queenVisual;

    protected new void Awake() {
        chessPieceVisual = queenVisual;
        base.Awake();
    }
}
