using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;

    [Header("State")]
    public int totalMoney = 0;

    [HideInInspector] public UnityEvent<int> OnTurnScoreCalculated;
    [HideInInspector] public UnityEvent<int> OnMoneyChanged;

    void Awake() {
        Instance = this;
    }

    public IEnumerator CalculateScoreCoroutine(Board board) {
        int totalThreatened = 0;
        int overlapWeight = 1;

        foreach (Cell cell in board.mAllCells) {
            if (cell == null) continue;

            int t = cell.mThreatCount;

            if (t <= 0) continue;

            totalThreatened++;

            if (t >= 2)
                overlapWeight += t - 1;
        }

        int turnScore = totalThreatened * overlapWeight;

        yield return null;

        AddMoney(turnScore);
        OnTurnScoreCalculated?.Invoke(turnScore);

        Debug.Log($"[ScoreManager] Threatened: {totalThreatened} | OverlapWeight: {overlapWeight} | Turn Score: {turnScore} | Total Money: {totalMoney}");
    }

    public void AddMoney(int amount) {
        totalMoney += amount;
        OnMoneyChanged?.Invoke(totalMoney);
    }


    public bool SpendMoney(int amount) {
        if (amount > totalMoney) {
            Debug.LogWarning($"[ScoreManager] Not enough money. Has: {totalMoney}, Needs: {amount}");
            return false;
        }
        totalMoney -= amount;
        OnMoneyChanged?.Invoke(totalMoney);
        return true;
    }

    public bool CanAfford(int amount) => totalMoney >= amount;
}