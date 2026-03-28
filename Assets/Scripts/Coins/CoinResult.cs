using System;
using UnityEngine;

public class CoinResult : MonoBehaviour {

    public float angleToleranceDegrees = 25f;

    public event Action<bool> OnResult;

    CoinFlattener _flattener;
    bool _resultFired;

    void Awake() {
        _flattener = GetComponent<CoinFlattener>();
    }

    void Update() {
        if (_resultFired) return;

        if (_flattener == null) {
            Debug.LogWarning($"{name} CoinResult: CoinFlattener not found on this coin!");
            return;
        }

        if (!_flattener.Settled) return;

        FireResult();
    }

    void FireResult() {
        _resultFired = true;

        float yRot = transform.eulerAngles.y;
        if (yRot > 180f) yRot -= 360f;

        bool isHeads = Mathf.Abs(yRot - (-90f)) < angleToleranceDegrees;
        bool isTails = Mathf.Abs(yRot - 90f) < angleToleranceDegrees;

        if (!isHeads && !isTails) {
            isHeads = yRot < 0f;
            Debug.LogWarning($"{name}: ambiguous Y rotation {yRot:F1} — defaulting to {(isHeads ? "heads" : "tails")}");
        }

        string result = isHeads ? "HEADS" : "TAILS";
        Debug.Log($"{name} result: {result} (Y rotation: {yRot:F1})");

        OnResult?.Invoke(isHeads);
        CoinEvents.FireResult(isHeads);
    }
}