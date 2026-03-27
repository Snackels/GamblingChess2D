using UnityEngine;

/// <summary>
/// Added automatically by CoinSpawner at runtime.
/// Notifies CoinSpawner when this coin is destroyed so the active list stays clean.
/// You don't need to add this to the prefab — it's injected on spawn.
/// </summary>
public class CoinLifetime : MonoBehaviour {
    private CoinSpawner _spawner;

    public void Init(CoinSpawner spawner) {
        _spawner = spawner;
    }

    private void OnDestroy() {
        _spawner?.UnregisterCoin(gameObject);
    }
}