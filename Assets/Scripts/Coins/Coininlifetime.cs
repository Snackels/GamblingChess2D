using UnityEngine;

public class CoinLifetime : MonoBehaviour {
    CoinSpawner _spawner;

    public void Init(CoinSpawner spawner) {
        _spawner = spawner;
    }

    void OnDestroy() {
        _spawner?.UnregisterCoin(gameObject);
    }
}