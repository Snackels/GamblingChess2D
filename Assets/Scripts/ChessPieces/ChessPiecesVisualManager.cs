using UnityEngine;

public class ChessPiecesVisualManager : MonoBehaviour {
    public static ChessPiecesVisualManager Instance;

    public Vector3 MouseWorldPos { get; private set; }
    Camera mainCamera;
    Canvas canvas;

    void Awake() {
        Instance = this;
        mainCamera = Camera.main;
    }

    public void SetCanvas(Canvas c) {
        canvas = c;
    }

    void Update() {
        Vector2 mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        MouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0)
        );
    }

    public Vector3 UIToWorldPosition(Vector3 uiPosition, float fixedZ) {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiPosition);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPoint.x, screenPoint.y, 0)
        );
        worldPos.z = fixedZ;
        return worldPos;
    }
}