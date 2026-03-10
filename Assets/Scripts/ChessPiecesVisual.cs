using UnityEngine;
using DG.Tweening;

public class ChessPieceVisual : MonoBehaviour {
    private bool initialized = false;

    [Header("Chess Piece")]
    public ChessPieces parentPiece;
    private Transform pieceTransform;
    private Vector3 rotationDelta;
    private Vector3 movementDelta;
    private Canvas canvas;

    [Header("References")]
    [SerializeField] public Transform shakeParent;
    [SerializeField] public Transform tiltParent;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 12;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float manualTiltAmount = 25;
    [SerializeField] private float autoTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHover = 1.15f;
    [SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = .15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;

    [SerializeField] private float dragZOffset = -1f;
    float fixedZ;

    public void Initialize(ChessPieces piece) {
        parentPiece = piece;
        pieceTransform = piece.transform;
        canvas = piece.GetComponentInParent<Canvas>();
        fixedZ = transform.position.z;

        parentPiece.PointerEnterEvent.AddListener(PointerEnter);
        parentPiece.PointerExitEvent.AddListener(PointerExit);
        parentPiece.BeginDragEvent.AddListener(BeginDrag);
        parentPiece.EndDragEvent.AddListener(EndDrag);

        initialized = true;
    }

    void Update() {
        if (!initialized || parentPiece == null) return;
        SmoothFollow();
        FollowRotation();
        PieceTilt();
    }

    private void SmoothFollow() {
        Vector3 targetWorldPos = UIToWorldPosition(pieceTransform.position);
        targetWorldPos.z = parentPiece.isDragging ? fixedZ + dragZOffset : fixedZ;
        transform.position = Vector3.Lerp(transform.position, targetWorldPos, followSpeed * Time.deltaTime);
    }

    private Vector3 UIToWorldPosition(Vector3 uiPosition) {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiPosition);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        worldPos.z = fixedZ;
        return worldPos;
    }

    private void FollowRotation() {
        Vector3 movement = transform.position - pieceTransform.position;
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (parentPiece.isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void PieceTilt() {
        // stop wobble when dragging
        float sine = parentPiece.isDragging ? 0 : Mathf.Sin(Time.time) * (parentPiece.isHovering ? .2f : 1);
        float cosine = parentPiece.isDragging ? 0 : Mathf.Cos(Time.time) * (parentPiece.isHovering ? .2f : 1);

        Vector2 mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        mouseWorldPos.z = fixedZ;

        Vector3 offset = transform.position - mouseWorldPos;

        float tiltX = parentPiece.isDragging ? (movementDelta.y * -manualTiltAmount) : parentPiece.isHovering ? (offset.y * -1 * manualTiltAmount) : 0;
        float tiltY = parentPiece.isDragging ? (movementDelta.x * manualTiltAmount) : parentPiece.isHovering ? (offset.x * manualTiltAmount) : 0;
        float tiltZ = 0;

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void BeginDrag(ChessPieces piece) {
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
    }

    private void EndDrag(ChessPieces piece) {
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void PointerEnter(ChessPieces piece) {
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(ChessPieces piece) {
        if (!parentPiece.wasDragged)
            transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }
}