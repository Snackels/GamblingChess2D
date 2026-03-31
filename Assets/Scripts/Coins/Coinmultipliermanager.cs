using UnityEngine;
using System.Collections;

public class CoinMultiplierManager : MonoBehaviour {
    public static CoinMultiplierManager Instance;

    [Header("References")]
    public CoinMaster coinMaster;
    public ScoreManager scoreManager;

    [Header("Coin Cost")]
    [Tooltip("Money deducted per coin thrown.")]
    public float coinCostPerThrow = 10f;

    float _pendingMultiplier = 1f;
    int _pendingOverlapDelta = 0;

    public float PendingMultiplier => _pendingMultiplier;
    public int PendingOverlapDelta => _pendingOverlapDelta;

    static readonly float[] AllHeadsMultiplier = { 1.25f, 2f, 3f, 4f, 5f };   
    static readonly float[] AllTailsMultiplier = { 0.8f, 0.7f, 0.6f, 0.5f, 0f };


    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        if (coinMaster != null)
            coinMaster.OnSessionComplete += HandleSessionComplete;
    }

    void OnDisable() {
        if (coinMaster != null)
            coinMaster.OnSessionComplete -= HandleSessionComplete;
    }


    public bool CanAffordThrow(int count) {
        float cost = coinCostPerThrow * count;
        return scoreManager != null && scoreManager.CanAfford(cost);
    }

    public bool TrySpendCoinCost(int count) {
        if (scoreManager == null) return false;
        float cost = coinCostPerThrow * count;
        return scoreManager.SpendMoney(cost);
    }


    public (float multiplier, int overlapDelta) ConsumeResult() {
        float m = _pendingMultiplier;
        int d = _pendingOverlapDelta;
        ResetPending();
        return (m, d);
    }

    public void ResetPending() {
        _pendingMultiplier = 1f;
        _pendingOverlapDelta = 0;
    }

    public bool TryRegisterThrow(int coinCount) {
        if (scoreManager == null) return false;
        float cost = coinCostPerThrow * coinCount;
        if (!scoreManager.CanAfford(cost)) {
            Log($"Cannot afford {coinCount} coins (cost {cost}, have {scoreManager.totalMoney})");
            return false;
        }
        scoreManager.SpendMoney(cost);
        Log($"Deducted {cost} for {coinCount} coins");
        return true;
    }

    void HandleSessionComplete(int heads, int tails) {
        int total = heads + tails;
        if (total == 0) return;

        int idx = Mathf.Clamp(total - 1, 0, 4);   // 0-based index for lookup tables

        bool allHeads = (heads == total);
        bool allTails = (tails == total);

        if (allHeads) {
            _pendingMultiplier = AllHeadsMultiplier[idx];
            _pendingOverlapDelta = 0;
            Log($"All Heads ({total} coins) → ×{_pendingMultiplier}");
            return;
        }

        if (allTails) {
            _pendingMultiplier = AllTailsMultiplier[idx];
            _pendingOverlapDelta = 0;
            Log($"All Tails ({total} coins) → ×{_pendingMultiplier}");
            return;
        }

        _pendingMultiplier = 1f;   // neutral score multiplier for mixed

        switch (total) {
            case 1:
                // Only one coin → can't be mixed, handled above; safety fallback
                _pendingOverlapDelta = 0;
                break;

            case 2:
                // 1H / 1T → tie → reset multiplier to ×1 (already set)
                _pendingOverlapDelta = 0;
                Log("2-coin Tie → multiplier reset to ×1");
                break;

            case 3:
                _pendingOverlapDelta = (heads > tails) ? +1 : -1;
                Log($"3-coin Mixed ({heads}H/{tails}T) → overlap delta {_pendingOverlapDelta:+0;-0}");
                break;

            case 4:
                if (heads == tails) {
                    // 2H/2T tie → refund the 4-coin cost, nothing else happens
                    _pendingOverlapDelta = 0;
                    if (scoreManager != null) scoreManager.AddMoney(coinCostPerThrow * 4f);
                    Log("4-coin Tie (2H/2T) → full refund, no effect");
                }
                else {
                    _pendingOverlapDelta = (heads > tails) ? +2 : -2;
                    Log($"4-coin Mixed ({heads}H/{tails}T) → overlap delta {_pendingOverlapDelta:+0;-0}");
                }
                break;

            case 5:
                // Majority heads → +headCount, majority tails → -tailCount
                _pendingOverlapDelta = (heads > tails) ? +heads : -tails;
                Log($"5-coin Mixed ({heads}H/{tails}T) → overlap delta {_pendingOverlapDelta:+0;-0}");
                break;

            default:
                // >5 coins: clamp to 5-coin rules
                _pendingOverlapDelta = (heads > tails) ? +heads : -tails;
                break;
        }
    }

    static void Log(string msg) =>
        Debug.Log($"[CoinMultiplierManager] {msg}");
}