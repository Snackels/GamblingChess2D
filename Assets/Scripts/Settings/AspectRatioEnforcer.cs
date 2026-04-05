using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectRatioEnforcer : MonoBehaviour {

    [Header("Target Aspect Ratio")]
    public int targetWidth = 9;
    public int targetHeight = 16;

    Camera _cam;

    void Awake() => _cam = GetComponent<Camera>();

    void Start() {
        _cam = GetComponent<Camera>();
        Apply();
    }

    void OnEnable() => Apply();

#if UNITY_EDITOR
    void Update() => Apply(); // live preview in Editor
#endif

    void Apply() {
        if (_cam == null) _cam = GetComponent<Camera>();

        float targetAspect = (float)targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(currentAspect, targetAspect)) {
            // Perfect match — full viewport
            _cam.rect = new Rect(0, 0, 1, 1);
            return;
        }

        if (currentAspect > targetAspect) {
            // Screen is wider than target → pillarbox (black bars left/right)
            float normalizedWidth = targetAspect / currentAspect;
            float barWidth = (1f - normalizedWidth) / 2f;
            _cam.rect = new Rect(barWidth, 0, normalizedWidth, 1);
        }
        else {
            // Screen is taller than target → letterbox (black bars top/bottom)
            float normalizedHeight = currentAspect / targetAspect;
            float barHeight = (1f - normalizedHeight) / 2f;
            _cam.rect = new Rect(0, barHeight, 1, normalizedHeight);
        }
    }
}