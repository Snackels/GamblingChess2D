using UnityEngine;

[RequireComponent(typeof(CoinController))]
public class Coin : MonoBehaviour {

    float _gravity;
    float _maxFallSpeed = 150f;

    Vector3 _velocity;

    CoinController _controller;

    void Awake() {
        _controller = GetComponent<CoinController>();


    }
}
