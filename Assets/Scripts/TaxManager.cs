using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TaxManager : MonoBehaviour {
    public static TaxManager Instance;

    [SerializeField] float startingTax = 4f;
    float currentTax;

    void Awake() {
        Instance = this;
        currentTax = startingTax;
    }

    public void ChargeTax() {
        if (!ScoreManager.Instance.SpendMoney(currentTax)) {
            Debug.Log($"[TaxManager] Can't pay tax of {currentTax} — restarting.");
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        Debug.Log($"[TaxManager] Tax charged: {currentTax}");
        currentTax *= 1.3f;
    }

    public float GetCurrentTax() => currentTax;
    public float GetNextTax() => currentTax * 1.3f;
}