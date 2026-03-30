using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;

    [Header("State")]
    public float totalMoney = 150f;

    [HideInInspector] public UnityEvent<float> OnTurnScoreCalculated;
    [HideInInspector] public UnityEvent<float> OnMoneyChanged;

    void Awake() {
        Instance = this;
    }

    public IEnumerator CalculateScoreCoroutine(Board board) {
        float coinMultiplier = 1f;
        int overlapDelta = 0;

        if (CoinMultiplierManager.Instance != null) {
            (coinMultiplier, overlapDelta) = CoinMultiplierManager.Instance.ConsumeResult();
        }

        Cell highestCell = null;
        int highestCount = 0;

        if (overlapDelta != 0) {
            foreach (Cell cell in board.mAllCells) {
                if (cell == null) continue;
                if (cell.mThreatCount > highestCount) {
                    highestCount = cell.mThreatCount;
                    highestCell = cell;
                }
            }
        }

        int totalThreatened = 0;
        int overlapWeight = 0;

        foreach (Cell cell in board.mAllCells) {
            if (cell == null) continue;

            int t = cell.mThreatCount;

            if (overlapDelta != 0 && cell == highestCell && highestCount > 1)
                t = Mathf.Max(1, t + overlapDelta);

            if (t <= 0) continue;

            if (t == 1)
                totalThreatened++;
            else
                overlapWeight += t;
        }

        if (overlapWeight == 0) overlapWeight = 1;

        float baseScore = totalThreatened * overlapWeight;

        if (overlapDelta != 0 && highestCell != null && highestCount > 1)
            Debug.Log($"[ScoreManager] Coin overlap delta {overlapDelta:+0;-0} on highest cell ({highestCount}) → scoring only, display unchanged");

        float turnScore = baseScore * coinMultiplier;

        yield return null;

        AddMoney(turnScore);
        OnTurnScoreCalculated?.Invoke(turnScore);

        Debug.Log($"[ScoreManager] Threatened: {totalThreatened} | OverlapWeight: {overlapWeight} | BaseScore: {baseScore} | CoinMult: ×{coinMultiplier} | Turn Score: {turnScore} | Total: {totalMoney}");
    }

    public void AddMoney(float amount) {
        totalMoney += amount;
        OnMoneyChanged?.Invoke(totalMoney);
    }


    public bool SpendMoney(float amount) {
        if (amount > totalMoney) {
            Debug.LogWarning($"[ScoreManager] Not enough money. Has: {totalMoney}, Needs: {amount}");
            return false;
        }
        totalMoney -= amount;
        OnMoneyChanged?.Invoke(totalMoney);
        return true;
    }

    public bool CanAfford(float amount) => totalMoney >= amount;
}