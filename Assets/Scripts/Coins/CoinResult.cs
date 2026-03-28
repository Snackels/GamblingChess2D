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

    void Awake() {
        _flattener = GetComponent<CoinFlattener>();
        _rb = GetComponent<Rigidbody>();

        if (_flattener == null)
            Debug.LogWarning($"{name} CoinResult: CoinFlattener not found!");
    }

    void Update() {
        if (_resultFired) return;
        if (_flattener == null || !_flattener.Settled) return;

        _resultFired = true;
        StartCoroutine(WaitUntilTrulyFlat());
    }

    IEnumerator WaitUntilTrulyFlat() {
        float sustainedTimer = 0f;

        while (sustainedTimer < sustainedSettleTime) {
            bool isAngularlyStill = _rb == null || _rb.angularVelocity.magnitude < angularStillThreshold;
            bool isLinearlyStill = _rb == null || _rb.linearVelocity.magnitude < linearStillThreshold;
            bool isFlat = Mathf.Abs(Vector3.Dot(transform.right, Vector3.forward)) >= flatDotThreshold;

            if (isAngularlyStill && isLinearlyStill && isFlat) {
                sustainedTimer += Time.deltaTime;
            }
            else {
                sustainedTimer = 0f;
            }

            yield return null;
        }

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