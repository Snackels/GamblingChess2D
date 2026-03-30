using UnityEngine;
using System;
using TMPro;

public class CoinMaster : MonoBehaviour {
    [Header("References")]
    public CoinSpawner coinSpawner;

    [Header("UI")]
    public TextMeshProUGUI headsText;
    public TextMeshProUGUI tailsText;
    public TextMeshProUGUI winnerText;

    int _heads;
    int _tails;
    int _pending;
    int _total;

    int _sessionId;
    public int CurrentSessionId => _sessionId;

    public event Action<int, int> OnSessionComplete;

    void OnEnable() {
        CoinEvents.OnCoinResult += HandleCoinResult;
    }

    void OnDisable() {
        CoinEvents.OnCoinResult -= HandleCoinResult;
    }

    public void RegisterThrow(int coinCount) {
        _sessionId++;
        coinSpawner?.ClearCoins(silent: true);
        _heads = 0;
        _tails = 0;
        _pending = coinCount;
        _total = coinCount;

        if (headsText != null) headsText.text = "0";
        if (tailsText != null) tailsText.text = "0";
        if (winnerText != null) winnerText.text = "...";
    }

    void HandleCoinResult(bool isHeads) {
        if (isHeads) _heads++;
        else _tails++;

        _pending = Mathf.Max(0, _total - _heads - _tails);

        if (headsText != null) headsText.text = _heads.ToString();
        if (tailsText != null) tailsText.text = _tails.ToString();

        if (_pending == 0 && _total > 0)
            SessionComplete();
    }

    void SessionComplete() {
        string winner = _heads > _tails ? "Heads wins!" : _tails > _heads ? "Tails wins!" : "Tie!";

        if (winnerText != null) winnerText.text = winner;

        OnSessionComplete?.Invoke(_heads, _tails);
    }
}