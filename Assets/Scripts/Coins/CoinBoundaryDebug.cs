using UnityEngine;
/// <summary>
/// Attach to the same GameObject as CoinBoundary.
/// Press Play and read the Console — it tells you exactly what is misconfigured.
/// Remove this script once everything works.
/// </summary>
public class CoinBoundaryDebug : MonoBehaviour {
    [Tooltip("Must match the Coin layer number in Project Settings")]
    public int coinLayer = 8;

    [Tooltip("Must match the CoinBoundary layer number in Project Settings")]
    public int coinBoundaryLayer = 7;

    private void Start() {
        Debug.Log("=== CoinBoundary Debug ===");

        // 1. Check wall root exists and is active
        Transform wallRoot = transform.Find("_CoinBoundaryWalls");
        if (wallRoot == null) {
            Debug.LogError("FAIL: _CoinBoundaryWalls not found. " +
                "CoinBoundary.Awake() may not have run — is the GameObject active?");
            return;
        }

        Debug.Log($"Wall root active: {wallRoot.gameObject.activeSelf}  " +
                  $"(should be TRUE if panel is open, FALSE if closed)");

        // 2. Check each wall exists, has a collider, and is on the right layer
        string[] wallNames = { "Wall_L", "Wall_R", "Wall_Top", "Wall_Bot", "Wall_Floor" };
        foreach (string wn in wallNames) {
            Transform w = wallRoot.Find(wn);
            if (w == null) {
                Debug.LogError($"FAIL: {wn} not found under _CoinBoundaryWalls");
                continue;
            }

            BoxCollider col = w.GetComponent<BoxCollider>();
            if (col == null) {
                Debug.LogError($"FAIL: {wn} has no BoxCollider");
                continue;
            }

            bool layerOk = w.gameObject.layer == coinBoundaryLayer;
            Debug.Log($"{wn} | layer={w.gameObject.layer} (want {coinBoundaryLayer}) {(layerOk ? "OK" : "WRONG")} " +
                      $"| size={col.size} | pos={w.position}");
        }

        // 3. Check layer collision matrix
        bool canCollide = !Physics.GetIgnoreLayerCollision(coinLayer, coinBoundaryLayer);
        if (!canCollide)
            Debug.LogError($"FAIL: Layer Collision Matrix has Coin({coinLayer}) vs " +
                           $"CoinBoundary({coinBoundaryLayer}) set to IGNORE. " +
                           $"Go to Edit > Project Settings > Physics > Layer Collision Matrix and turn it ON.");
        else
            Debug.Log($"Layer matrix Coin({coinLayer}) vs CoinBoundary({coinBoundaryLayer}): OK (collisions enabled)");

        // 4. Check for any active coins and their layers
        CoinGravity[] coins = FindObjectsByType<CoinGravity>(FindObjectsSortMode.None);
        if (coins.Length == 0) {
            Debug.Log("No coins spawned yet — spawn one and check the Console again.");
        }
        else {
            foreach (var c in coins) {
                bool coinLayerOk = c.gameObject.layer == coinLayer;
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Debug.Log($"Coin '{c.gameObject.name}' | layer={c.gameObject.layer} (want {coinLayer}) " +
                          $"{(coinLayerOk ? "OK" : "WRONG LAYER — fix on prefab")} " +
                          $"| useGravity={rb?.useGravity} (should be false) " +
                          $"| collisionDetection={rb?.collisionDetectionMode} (want Continuous)");
            }
        }

        Debug.Log("=== End Debug ===");
    }
}