using TMPro;
using UnityEngine;

public class UpkeepDisplay : MonoBehaviour {
    [SerializeField] ChessPieces piece;
    TextMeshProUGUI tmp;

    void Awake() {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        piece.OnPiecePlaced += OnPiecePlaced;
        piece.OnPieceSold += OnPieceSold;
        ScoreManager.Instance.OnTurnScoreCalculated.AddListener(OnTurnEnd);
        tmp.enabled = false;
    }

    void OnPiecePlaced(ChessPieces p) {
        tmp.enabled = true;
        UpdateDisplay();
    }
    void OnTurnEnd(float score) => UpdateDisplay();

    void UpdateDisplay() {
        tmp.text = $"{piece.GetUpkeepCost():F2}";
    }

    void OnPieceSold(ChessPieces p) => tmp.enabled = false;

    void OnDestroy() {
        piece.OnPiecePlaced -= OnPiecePlaced;
        piece.OnPieceSold -= OnPieceSold;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnTurnScoreCalculated.RemoveListener(OnTurnEnd);
    }
}

