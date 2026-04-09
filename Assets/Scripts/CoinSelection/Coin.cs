using UnityEngine;

public class Coin : CoinPiece {
    [SerializeField] private CoinMovement coinVisual;

    protected new void Awake() {
        coinMovement = coinVisual;
        base.Awake();
    }
}
