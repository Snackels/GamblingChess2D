using UnityEngine;
using DG.Tweening;

public class TaxManager : MonoBehaviour {
    public static TaxManager Instance;
    [SerializeField] GameOverScreen gameOverScreen;

    [SerializeField] float baseTax = 8f;
    [SerializeField] float taxPerRound = 4f;

    void Awake() {
        Instance = this;
    }

    float CalculateTax(int round) => baseTax + (round * taxPerRound);

    public void ChargeTax() {
        int currentRound = TurnManager.Instance.currentTurn;
        float tax = CalculateTax(currentRound);

        if (!ScoreManager.Instance.SpendMoney(tax)) {
            Debug.Log($"[TaxManager] Can't pay tax of {tax} — restarting.");
            DOTween.KillAll();
            gameOverScreen.GameOver();
            return;
        }

        Debug.Log($"[TaxManager] Tax charged: {tax}");
    }

    public float GetCurrentTax() => CalculateTax(TurnManager.Instance.currentTurn);
    public float GetNextTax() => CalculateTax(TurnManager.Instance.currentTurn + 1);
}