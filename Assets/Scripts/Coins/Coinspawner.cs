using UnityEngine;
using TMPro;
using System.Collections;

public class CoinSpawner : MonoBehaviour {
    [Header("References")]
    [Tooltip("Coin prefab — Rigidbody, Collider, CoinResult")]
    public GameObject coinPrefab;
    [Tooltip("Where the coin spawns — place this empty GameObject inside your boundary")]
    public Transform spawnPoint;
    [Tooltip("Reference to boundary — used to clamp spawn point inside bounds")]
    public CoinBoundary boundary;

    [Header("Batch Spawn UI")]
    [Tooltip("TMP text that displays the current coin count to throw")]
    public TextMeshProUGUI coinCountText;
    [Tooltip("How many coins to throw per spawn. Use AddCoin() and RemoveCoin() for +/- buttons")]
    public int coinCountToThrow = 1;
    [Tooltip("Minimum coins per throw")]
    public int minCoinsPerThrow = 1;
    [Tooltip("Maximum coins allowed per throw — caps the +/- buttons")]
    public int maxCoinsPerThrow = 10;
    [Tooltip("Seconds between each coin spawning in a batch — stagger so they don't all stack")]
    public float spawnStagger = 0.08f;

    [Header("Active Coin Limit")]
    [Tooltip("Max coins alive in the scene at once — oldest removed when limit hit")]
    public int maxCoinsAlive = 20;

    [Header("Z Gravity")]
    [Tooltip("How fast the coin accelerates toward the floor (+Z). Tune for feel.")]
    public float gravityForce = 15f;

    [Header("Throw Force")]
    [Tooltip("Direction the coin is thrown in world space. " +
             "(0,1,1) = up and forward, (0,0,1) = straight to floor, (1,0.5,1) = right arc.")]
    public Vector3 throwDirection = new Vector3(0f, 1f, 1f);
    [Tooltip("Min / Max throw speed — each coin picks a random value in this range")]
    public Vector2 throwSpeedRange = new Vector2(3f, 7f);
    [Tooltip("Random spread added to throw direction each toss")]
    public float throwDirectionVariance = 0.2f;

    [Header("Spin")]
    [Tooltip("Min / Max angular velocity — each coin picks a random value in this range")]
    public Vector2 spinSpeedRange = new Vector2(5f, 15f);
    [Tooltip("Randomize spin axis so each coin tumbles differently")]
    public bool randomSpinAxis = true;
    [Tooltip("Fixed spin axis if randomSpinAxis is off — (1,0,0) = real coin flip axis")]
    public Vector3 fixedSpinAxis = new Vector3(1f, 0f, 0f);

    [Header("Spawn Rotation")]
    public bool randomRotation = true;
    public Vector3 fixedRotation = Vector3.zero;

    [Header("Spawn Wobble")]
    [Tooltip("Random XY offset per coin so they don't all stack at the same point")]
    public float spawnJitter = 0.1f;

    private readonly System.Collections.Generic.List<GameObject> _activeCoins
        = new System.Collections.Generic.List<GameObject>();

    private bool _spawning = false;

    private void Start() {
        UpdateCountText();
    }

    public void AddCoin() {
        coinCountToThrow = Mathf.Min(coinCountToThrow + 1, maxCoinsPerThrow);
        UpdateCountText();
    }

    public void RemoveCoin() {
        coinCountToThrow = Mathf.Max(coinCountToThrow - 1, minCoinsPerThrow);
        UpdateCountText();
    }

    private void UpdateCountText() {
        if (coinCountText != null)
            coinCountText.text = coinCountToThrow.ToString();
    }

    public void SpawnCoins() {
        if (_spawning) return;
        if (coinPrefab == null || spawnPoint == null) {
            Debug.LogWarning("CoinSpawner: coinPrefab or spawnPoint not assigned.");
            return;
        }
        StartCoroutine(SpawnBatch(coinCountToThrow));
    }

    private IEnumerator SpawnBatch(int count) {
        _spawning = true;
        for (int i = 0; i < count; i++) {
            SpawnSingle();
            if (i < count - 1)
                yield return new WaitForSeconds(spawnStagger);
        }
        _spawning = false;
    }

    private void SpawnSingle() {
        // Enforce alive limit
        if (_activeCoins.Count >= maxCoinsAlive)
            RemoveOldestCoin();

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
            Vector3 dir = throwDirection.normalized + new Vector3(
                Random.Range(-throwDirectionVariance, throwDirectionVariance),
                Random.Range(-throwDirectionVariance, throwDirectionVariance),
                Random.Range(-throwDirectionVariance, throwDirectionVariance)
            );
            rb.linearVelocity = dir.normalized * Random.Range(throwSpeedRange.x, throwSpeedRange.y);

            Vector3 spinAxis = randomSpinAxis ? Random.onUnitSphere : fixedSpinAxis.normalized;
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

        Gizmos.color = new Color(1f, 1f, 0.2f, 0.9f);
        Gizmos.DrawWireSphere(spawnPoint.position, 0.08f);

        Gizmos.color = new Color(1f, 1f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(spawnPoint.position, spawnJitter);

        Vector3 from = spawnPoint.position;
        Vector3 baseDir = throwDirection.normalized;
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.5f);
        Gizmos.DrawLine(from, from + baseDir * Mathf.Clamp(throwSpeedRange.x * 0.15f, 0.2f, 2f));
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.95f);
        Vector3 maxTip = from + baseDir * Mathf.Clamp(throwSpeedRange.y * 0.15f, 0.3f, 2f);
        Gizmos.DrawLine(from, maxTip);
        Gizmos.DrawWireSphere(maxTip, 0.05f);

        Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.9f);
        Gizmos.DrawLine(from, from + Vector3.forward * 0.5f);
        Gizmos.DrawWireSphere(from + Vector3.forward * 0.5f, 0.04f);
    }
}