using UnityEngine;

public class ChessPieceVisual : MonoBehaviour {
    private ChessPieces chessPiece;

    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverSpeed = 10f;

    [Header("Tilt")]
    [SerializeField] private float tiltAmount = 20f;
    [SerializeField] private float tiltSpeed = 10f;

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.3f;
    [SerializeField] private float dragScale = 1.1f;
    [SerializeField] private float scaleSpeed = 10f;

    private Vector3 originalLocalPosition;
    private Vector3 originalScale;
    private Vector3 previousPosition;

    public void Initialize(ChessPieces piece) {
        chessPiece = piece;
        originalLocalPosition = transform.localPosition;
        originalScale = transform.localScale;
        previousPosition = transform.position;
    }

    void Update() {
        if (chessPiece == null) return;
        HandleHoverHeight();
        HandleTilt();
        HandleScale();
        previousPosition = transform.position;
    }

    void HandleHoverHeight() {
        float targetY = chessPiece.isHovering || chessPiece.isDragging
            ? originalLocalPosition.y + hoverHeight
            : originalLocalPosition.y;

        transform.localPosition = new Vector3(
            originalLocalPosition.x,
            Mathf.Lerp(transform.localPosition.y, targetY, hoverSpeed * Time.deltaTime),
            originalLocalPosition.z
        );
    }

    void HandleTilt() {
        Vector3 delta = transform.position - previousPosition;
        float tiltX = 0f;
        float tiltZ = 0f;

        if (chessPiece.isDragging) {
            tiltX = -delta.y * tiltAmount;
            tiltZ = -delta.x * tiltAmount;
        }

        Quaternion targetRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            tiltSpeed * Time.deltaTime
        );
    }

    void HandleScale() {
        Vector3 targetScale = originalScale;
        if (chessPiece.isDragging)
            targetScale = originalScale * dragScale;
        else if (chessPiece.isHovering)
            targetScale = originalScale * hoverScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            scaleSpeed * Time.deltaTime
        );
    }
}