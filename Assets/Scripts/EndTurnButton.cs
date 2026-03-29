using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour {
    Button button;

    void Awake() {
        button = GetComponent<Button>();
    }

    void Start() {
        GameManager.Instance.OnBoardChanged.AddListener(UpdateButton);
        UpdateButton();
    }

    void OnDestroy() {
        if (GameManager.Instance != null)
            GameManager.Instance.OnBoardChanged.RemoveListener(UpdateButton);
    }

    void UpdateButton() {
        button.interactable = GameManager.Instance.GetPiecesOnBoardCount() > 0;
    }
}