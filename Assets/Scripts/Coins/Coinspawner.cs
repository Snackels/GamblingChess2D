using UnityEngine;

/// <summary>
/// Spawns a coin at a chosen world position inside the boundary.
/// Coins fall in the +Z direction (toward the floor wall).
///
/// SETUP:
///   1. Attach this to any GameObject (e.g. CoinManager)
///   2. Assign CoinPrefab — needs Rigidbody + Collider + CoinResult
///   3. Create an empty GameObject "SpawnPoint" inside the boundary, assign it
///   4. On the coin Rigidbody: set Use Gravity = OFF
///      We apply our own Z gravity so it falls toward the floor wall
///   5. Wire your UI Button OnClick → CoinSpawner.SpawnCoin()
///
/// COIN RIGIDBODY SETTINGS (on the prefab):
///   - Use Gravity: OFF  (we drive gravity manually via Gravity Force below)
///   - Collision Detection: Continuous  (prevents tunneling through floor)
///   - Interpolate: Interpolate
///   - Constraints: nothing frozen
/// </summary>
public class CoinSpawner : MonoBehaviour {
    [Header("References")]
    [Tooltip("Coin prefab — Rigidbody, Collider, CoinResult")]
    public GameObject coinPrefab;

    [Tooltip("Where the coin spawns — place this empty GameObject inside your boundary")]
    public Transform spawnPoint;

    [Tooltip("Reference to boundary — used to clamp spawn point inside bounds")]
    public CoinBoundary boundary;

    [Header("Z Gravity")]
    [Tooltip("How fast the coin accelerates toward the floor (+Z). Tune for feel.")]
    public float gravityForce = 15f;

    [Tooltip("Initial Z velocity when spawned — gives a satisfying drop-in feel")]
    public float initialZVelocity = 2f;

    [Header("Spawn Rotation")]
    [Tooltip("Spawn with a random rotation so every coin looks different")]
    public bool randomRotation = true;

    [Tooltip("If not random, spawn with this exact rotation")]
    public Vector3 fixedRotation = Vector3.zero;

    [Header("Spawn Wobble")]
    [Tooltip("Small random XY offset from spawn point so coins don't stack perfectly")]
    public float spawnJitter = 0.1f;

    [Header("Active Coins")]
    [Tooltip("Max coins alive at once — older coins are removed when limit is hit")]
    public int maxCoins = 20;

    private readonly System.Collections.Generic.List<GameObject> _activeCoins
        = new System.Collections.Generic.List<GameObject>();

    // ── public — wire to button ─────────────────────────────────────────────
    public void SpawnCoin() {
        if (coinPrefab == null || spawnPoint == null) {
            Debug.LogWarning("CoinSpawner: coinPrefab or spawnPoint not assigned.");
            return;
        }

        // Enforce max coin limit
        if (_activeCoins.Count >= maxCoins)
            RemoveOldestCoin();

        // Position with small jitter
        Vector3 pos = spawnPoint.position + new Vector3(
            Random.Range(-spawnJitter, spawnJitter),
            Random.Range(-spawnJitter, spawnJitter),
            0f
        );

        // Clamp inside boundary XY if boundary is assigned
        if (boundary != null)
            pos = boundary.ClampInsideXY(pos);

        Quaternion rot = randomRotation
            ? Random.rotation
            : Quaternion.Euler(fixedRotation);

        GameObject coin = Instantiate(coinPrefab, pos, rot);
        _activeCoins.Add(coin);

        // Give initial Z velocity
        Rigidbody rb = coin.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = new Vector3(0f, 0f, initialZVelocity);

        // Track for cleanup
        coin.AddComponent<CoinLifetime>().Init(this);
    }

    // called by CoinLifetime when a coin is destroyed/returned
    public void UnregisterCoin(GameObject coin) {
        _activeCoins.Remove(coin);
    }

    private void RemoveOldestCoin() {
        if (_activeCoins.Count == 0) return;
        GameObject oldest = _activeCoins[0];
        _activeCoins.RemoveAt(0);
        if (oldest != null) Destroy(oldest);
    }

    // ── gizmo — shows spawn point and jitter radius ─────────────────────────
    private void OnDrawGizmos() {
        if (spawnPoint == null) return;

        Gizmos.color = new Color(1f, 1f, 0.2f, 0.9f);
        Gizmos.DrawWireSphere(spawnPoint.position, 0.08f);

        Gizmos.color = new Color(1f, 1f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(spawnPoint.position, spawnJitter);

        // Arrow pointing +Z (fall direction)
        Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.9f);
        Vector3 from = spawnPoint.position;
        Vector3 to = from + Vector3.forward * 0.5f;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(to, 0.04f);
    }
}