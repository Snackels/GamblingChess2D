using UnityEngine;

public class Knight : ChessPieces
{
    [SerializeField] private ChessPieceVisual knightVisual;

    protected new void Awake()
    {
        chessPieceVisual = knightVisual;
        base.Awake();
    }
}
