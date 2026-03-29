using UnityEngine;
using TMPro;

public class TaxDisplay : MonoBehaviour {
    TextMeshProUGUI tmp;

    void Awake() {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        ScoreManager.Instance.OnTurnScoreCalculated.AddListener(OnTurnEnd);
        UpdateDisplay();
    }

    void OnDestroy() {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnTurnScoreCalculated.RemoveListener(OnTurnEnd);
    }

    void OnTurnEnd(float score) => UpdateDisplay();

    void UpdateDisplay() {
        tmp.text = $"{TaxManager.Instance.GetCurrentTax():0.##}";
    }
}