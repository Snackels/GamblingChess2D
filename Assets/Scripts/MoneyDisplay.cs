using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour {
    TextMeshProUGUI tmp;

    void Start() {
        tmp = GetComponent<TextMeshProUGUI>();
        ScoreManager.Instance.OnMoneyChanged.AddListener(UpdateDisplay);
        UpdateDisplay(ScoreManager.Instance.totalMoney);
    }

    void OnDestroy() {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnMoneyChanged.RemoveListener(UpdateDisplay);
    }

    void UpdateDisplay(float amount) {
        tmp.text = $"{amount}";
    }
}


