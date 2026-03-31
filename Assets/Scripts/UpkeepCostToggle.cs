using UnityEngine;
using TMPro;

public class UpkeepCostToggle : MonoBehaviour {

    [SerializeField] UpkeepDisplay upkeepDisplay;

    TextMeshProUGUI _tmp;

    void Awake() {
        _tmp = upkeepDisplay.GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        upkeepDisplay.OnUpkeepChanged += Refresh;
        Refresh(upkeepDisplay.CurrentUpkeep);
    }

    void Refresh(float upkeep) {
        gameObject.SetActive(upkeep > 0f);
    }

    void OnDestroy() {
        if (upkeepDisplay != null)
            upkeepDisplay.OnUpkeepChanged -= Refresh;
    }
}