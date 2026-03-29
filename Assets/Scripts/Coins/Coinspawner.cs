using UnityEngine;
using TMPro;
using System.Collections;

public class CoinSpawner : MonoBehaviour {
    [Header("References")]
    public GameObject coinPrefab;
    public Transform spawnPoint;
    public CoinBoundary boundary;
    public CoinMaster coinMaster;

    [Header("Batch Spawn UI")]
    public TextMeshProUGUI coinCountText;
    public int coinCountToThrow = 1;
    public int minCoinsPerThrow = 1;
    public int maxCoinsPerThrow = 10;
    public float spawnStagger = 0.08f;

    [Header("Active Coin Limit")]
    public int maxCoinsAlive = 20;

    [Header("Z Gravity")]
    public float gravityForce = 15f;

    [Header("Throw Force")]
    public Vector3 throwDirection = new Vector3(0f, 1f, 1f);
    public Vector2 throwSpeedRange = new Vector2(3f, 7f);
    public float throwDirectionVariance = 0.2f;

    [Header("Spin")]
    public Vector2 spinSpeedRange = new Vector2(5f, 15f);
    public bool randomSpinAxis = true;
    public Vector3 fixedSpinAxis = new Vector3(1f, 0f, 0f);

    [Header("Spawn Rotation")]
    public bool randomRotation = true;
    public Vector3 fixedRotation = Vector3.zero;

    [Header("Spawn Wobble")]
    public float spawnJitter = 0.1f;

    readonly System.Collections.Generic.List<GameObject> _activeCoins = new System.Collections.Generic.List<GameObject>();

    bool _spawning = false;

    void Start() {
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

    void UpdateCountText() {
        if (coinCountText != null)
            coinCountText.text = coinCountToThrow.ToString();
    }

    public void SpawnCoins() {
        if (_spawning) return;
        if (coinPrefab == null || spawnPoint == null) {
            Debug.LogWarning("CoinSpawner: coinPrefab or spawnPoint not assigned.");
            return;
        }

        // Block throw if player can't afford it
        if (CoinMultiplierManager.Instance != null) {
            if (!CoinMultiplierManager.Instance.TryRegisterThrow(coinCountToThrow))
                return;
        }

        StartCoroutine(SpawnBatch(coinCountToThrow));
    }

    IEnumerator SpawnBatch(int count) {
        _spawning = true;
        coinMaster?.RegisterThrow(count);
        for (int i = 0; i < count; i++) {
            SpawnSingle();
            if (i < count - 1)
                yield return new WaitForSeconds(spawnStagger);
        }
        _spawning = false;
    }

    void SpawnSingle() {
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

        // Stamp the coin with the current session so stale results are ignored
        CoinResult coinResult = coin.GetComponent<CoinResult>();
        if (coinResult != null) coinResult.Init(coinMaster);

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

    public void ClearCoins() {
        var copy = new System.Collections.Generic.List<GameObject>(_activeCoins);
        foreach (var coin in copy)
            if (coin != null) Destroy(coin);
        _activeCoins.Clear();
    }

    void RemoveOldestCoin() {
        if (_activeCoins.Count == 0) return;
        GameObject oldest = _activeCoins[0];
        _activeCoins.RemoveAt(0);
        if (oldest != null) Destroy(oldest);
    }

    void OnDrawGizmos() {
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