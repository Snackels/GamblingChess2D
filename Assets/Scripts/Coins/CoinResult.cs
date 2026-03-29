using UnityEngine;
using System;
using System.Collections;

public class CoinResult : MonoBehaviour {

    public float sustainedSettleTime = 0.4f;
    public float angularStillThreshold = 0.05f;
    public float linearStillThreshold = 0.05f;

    [Range(0f, 1f)]
    public float flatDotThreshold = 0.85f;

    public event Action<bool> OnResult;

    CoinFlattener _flattener;
    Rigidbody _rb;
    bool _resultFired;
    bool _coroutineStarted;

    // Stamped at spawn time — result is discarded if session has moved on
    CoinMaster _coinMaster;
    int _mySessionId;

    void Awake() {
        _flattener = GetComponent<CoinFlattener>();
        _rb = GetComponent<Rigidbody>();

        if (_flattener == null)
            Debug.LogWarning($"{name} CoinResult: CoinFlattener not found!");
    }

    /// <summary>Call immediately after spawning to bind this coin to the current session.</summary>
    public void Init(CoinMaster coinMaster) {
        _coinMaster = coinMaster;
        _mySessionId = coinMaster != null ? coinMaster.CurrentSessionId : -1;
    }

    bool IsSessionStale() =>
        _coinMaster != null && _coinMaster.CurrentSessionId != _mySessionId;

    void Update() {
        if (_resultFired || _coroutineStarted) return;
        if (IsSessionStale()) return;
        if (_flattener == null || !_flattener.Settled) return;

        _coroutineStarted = true;
        StartCoroutine(WaitUntilTrulyFlat());
    }

    IEnumerator WaitUntilTrulyFlat() {
        // Wait one extra frame so ForceFlat()'s rotation is fully applied
        yield return null;

        float sustainedTimer = 0f;

        while (sustainedTimer < sustainedSettleTime) {
            // Abort if this coin belongs to a dead session
            if (IsSessionStale()) yield break;

            bool isAngularlyStill = _rb == null || _rb.angularVelocity.magnitude < angularStillThreshold;
            bool isLinearlyStill = _rb == null || _rb.linearVelocity.magnitude < linearStillThreshold;
            bool isFlat = Mathf.Abs(Vector3.Dot(transform.right, Vector3.forward)) >= flatDotThreshold;

            if (isAngularlyStill && isLinearlyStill && isFlat)
                sustainedTimer += Time.deltaTime;
            else
                sustainedTimer = 0f;

            yield return null;
        }

        if (IsSessionStale()) yield break;

        _resultFired = true;
        FireResult();
    }

    void FireResult() {
        float dot = Vector3.Dot(transform.right, Vector3.forward);

        bool isHeads;

        if (Mathf.Abs(dot) >= flatDotThreshold) {
            isHeads = dot > 0f;
        }
        else {
            isHeads = dot > 0f;
            Debug.LogWarning($"{name}: ambiguous dot={dot:F3} — defaulting to {(isHeads ? "HEADS" : "TAILS")}");
        }

        OnResult?.Invoke(isHeads);
        CoinEvents.FireResult(isHeads);
    }
}