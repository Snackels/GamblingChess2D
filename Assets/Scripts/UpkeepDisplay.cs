using TMPro;
using UnityEngine;
using System;

public class UpkeepDisplay : MonoBehaviour {
    [SerializeField] ChessPieces piece;
    public ChessPieces Piece => piece;
    TextMeshProUGUI tmp;

    public event Action<float> OnUpkeepChanged;
    public float CurrentUpkeep { get; private set; }

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
        CurrentUpkeep = piece.GetUpkeepCost();
        tmp.text = $"{CurrentUpkeep:0.##}";
        OnUpkeepChanged?.Invoke(CurrentUpkeep);
    }

    void OnPieceSold(ChessPieces p) {
        tmp.enabled = false;
        CurrentUpkeep = 0f;
        OnUpkeepChanged?.Invoke(0f);
    }

    void OnDestroy() {
        piece.OnPiecePlaced -= OnPiecePlaced;
        piece.OnPieceSold -= OnPieceSold;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnTurnScoreCalculated.RemoveListener(OnTurnEnd);
    }
}
