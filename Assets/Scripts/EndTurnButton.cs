using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndTurnButton : MonoBehaviour {
    Button button;

    [Header("Score Breakdown TMP Labels")]
    [SerializeField] TextMeshProUGUI squaresControlledTMP;
    [SerializeField] TextMeshProUGUI squareOverlapTMP;
    [SerializeField] TextMeshProUGUI coinMultTMP;
    [SerializeField] TextMeshProUGUI moneyEarnedTMP;

    [Header("Coin Master Reference")]
    [SerializeField] CoinMaster coinMaster;

    void Awake() {
        button = GetComponent<Button>();
    }

    void Start() {
        GameManager.Instance.OnBoardChanged.AddListener(UpdateLive);
        ScoreManager.Instance.OnTurnScoreDetailed.AddListener(OnTurnScoreDetailed);

        if (coinMaster != null)
            coinMaster.OnSessionComplete += OnCoinSessionComplete;

        UpdateLive();

        if (moneyEarnedTMP != null) moneyEarnedTMP.text = "";
    }

    void OnDestroy() {
        if (GameManager.Instance != null)
            GameManager.Instance.OnBoardChanged.RemoveListener(UpdateLive);
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnTurnScoreDetailed.RemoveListener(OnTurnScoreDetailed);
        if (coinMaster != null)
            coinMaster.OnSessionComplete -= OnCoinSessionComplete;
    }

    // Wait one frame so CoinMultiplierManager processes the result first, then update
    void OnCoinSessionComplete(int heads, int tails) {
        StartCoroutine(UpdateLiveNextFrame());
    }

    IEnumerator UpdateLiveNextFrame() {
        yield return null;
        UpdateLive();
    }

    void UpdateLive() {
        button.interactable = GameManager.Instance.GetPiecesOnBoardCount() > 0;

        TurnScoreData preview = ScoreManager.Instance.GetLivePreview(GameManager.Instance.mBoard);

        if (squaresControlledTMP != null)
            squaresControlledTMP.text = $"{preview.squaresControlled}";

        if (squareOverlapTMP != null)
            squareOverlapTMP.text = $"{preview.overlapWeight}";

        if (coinMultTMP != null)
            coinMultTMP.text = $"x{preview.coinMultiplier:F2}";
    }

    void OnTurnScoreDetailed(TurnScoreData data) {
        if (moneyEarnedTMP != null)
            moneyEarnedTMP.text = $"+{data.moneyEarned:F2}";
    }
}