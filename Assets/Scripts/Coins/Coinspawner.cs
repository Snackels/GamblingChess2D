using UnityEngine;

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

    [Header("Throw Force")]
    [Tooltip("Direction the coin is thrown in world space. " +
             "Think of this like the angle of your hand when you toss — " +
             "(0,1,1) = forward and up, (0,0,1) = straight toward floor, (1,0.5,1) = right arc. " +
             "Does not need to be normalized.")]
    public Vector3 throwDirection = new Vector3(0f, 1f, 1f);

    [Tooltip("How hard the coin is thrown. Higher = faster, travels further before settling.")]
    public Vector2 throwSpeedRange = new Vector2(3f, 7f);

    [Tooltip("Randomize the throw direction slightly each toss so coins don't all go the same way")]
    public float throwDirectionVariance = 0.2f;

    [Header("Spin")]
    [Tooltip("How much angular velocity to apply on spawn — makes the coin tumble in the air")]
    public Vector2 spinSpeedRange = new Vector2(5f, 15f);

    [Tooltip("Randomize spin axis so each coin tumbles differently")]
    public bool randomSpinAxis = true;

    [Tooltip("Fixed spin axis if randomSpinAxis is off — (1,0,0) spins on X like a real coin flip")]
    public Vector3 fixedSpinAxis = new Vector3(1f, 0f, 0f);

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

    public void SpawnCoin() {
        if (coinPrefab == null || spawnPoint == null) {
            Debug.LogWarning("CoinSpawner: coinPrefab or spawnPoint not assigned.");
            return;
        }

        if (_activeCoins.Count >= maxCoins)
            RemoveOldestCoin();

        // Position with jitter
        Vector3 pos = spawnPoint.position + new Vector3(
            Random.Range(-spawnJitter, spawnJitter),
            Random.Range(-spawnJitter, spawnJitter),
            0f
        );
        if (boundary != null)
            pos = boundary.ClampInsideXY(pos);

        Quaternion rot = randomRotation
            ? Random.rotation
            : Quaternion.Euler(fixedRotation);

        GameObject coin = Instantiate(coinPrefab, pos, rot);
        _activeCoins.Add(coin);

        Rigidbody rb = coin.GetComponent<Rigidbody>();
        if (rb != null) {
            // Throw velocity — base direction + random variance
            Vector3 dir = throwDirection.normalized;
            dir += new Vector3(
                Random.Range(-throwDirectionVariance, throwDirectionVariance),
                Random.Range(-throwDirectionVariance, throwDirectionVariance),
                Random.Range(-throwDirectionVariance, throwDirectionVariance)
            );
            rb.linearVelocity = dir.normalized * Random.Range(throwSpeedRange.x, throwSpeedRange.y);

            // Spin — like the coin tumbling end-over-end
            Vector3 spinAxis = randomSpinAxis
                ? Random.onUnitSphere
                : fixedSpinAxis.normalized;
            rb.angularVelocity = spinAxis * Random.Range(spinSpeedRange.x, spinSpeedRange.y);
        }

        coin.AddComponent<CoinLifetime>().Init(this);
    }

    public void UnregisterCoin(GameObject coin) {
        _activeCoins.Remove(coin);
    }

    private void RemoveOldestCoin() {
        if (_activeCoins.Count == 0) return;
        GameObject oldest = _activeCoins[0];
        _activeCoins.RemoveAt(0);
        if (oldest != null) Destroy(oldest);
    }

    private void OnDrawGizmos() {
        if (spawnPoint == null) return;

        // Spawn point
        Gizmos.color = new Color(1f, 1f, 0.2f, 0.9f);
        Gizmos.DrawWireSphere(spawnPoint.position, 0.08f);

        // Jitter radius
        Gizmos.color = new Color(1f, 1f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(spawnPoint.position, spawnJitter);

        // Throw direction arrows — green, shows min and max throw range
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.5f);
        Vector3 from = spawnPoint.position;
        Vector3 baseDir = throwDirection.normalized;
        Gizmos.DrawLine(from, from + baseDir * Mathf.Clamp(throwSpeedRange.x * 0.15f, 0.2f, 2f));
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.95f);
        Vector3 maxTip = from + baseDir * Mathf.Clamp(throwSpeedRange.y * 0.15f, 0.3f, 2f);
        Gizmos.DrawLine(from, maxTip);
        Gizmos.DrawWireSphere(maxTip, 0.05f);

        // Z gravity arrow — orange, always +Z
        Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.9f);
        Gizmos.DrawLine(from, from + Vector3.forward * 0.5f);
        Gizmos.DrawWireSphere(from + Vector3.forward * 0.5f, 0.04f);
    }
}