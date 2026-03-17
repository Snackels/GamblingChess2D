using UnityEngine;
using DG.Tweening;

public class ChessPieceVisual : MonoBehaviour {
    bool initialized = false;

    [Header("Chess Piece")]
    public ChessPieces parentPiece;
    Transform pieceTransform;
    Vector3 rotationDelta;
    Vector3 movementDelta;
    Canvas canvas;

    [Header("References")]
    [SerializeField] public Transform shakeParent;
    [SerializeField] public Transform tiltParent;

    [Header("Follow Parameters")]
    [SerializeField] float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] float rotationAmount = 12;
    [SerializeField] float rotationSpeed = 20;
    [SerializeField] float manualTiltAmount = 25;
    [SerializeField] float autoTiltAmount = 20;
    [SerializeField] float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] bool scaleAnimations = true;
    [SerializeField] float scaleOnHover = 1.15f;
    [SerializeField] float scaleOnSelect = 1.25f;
    [SerializeField] float scaleTransition = .15f;
    [SerializeField] Ease scaleEase = Ease.OutBack;

    [Header("Hover Parameters")]
    [SerializeField] float hoverPunchAngle = 5;
    [SerializeField] float hoverTransition = .15f;

    [SerializeField] float dragZOffset = -1f;
    float fixedZ;

    Sequence deleteSequence;
    bool isDeleting = false;

    public bool IsInitialized => initialized;

    public void Initialize(ChessPieces piece) {
        parentPiece = piece;
        pieceTransform = piece.transform;
        canvas = piece.GetComponentInParent<Canvas>();
        fixedZ = transform.position.z;

        ChessPiecesVisualManager.Instance.SetCanvas(canvas);

        parentPiece.PointerEnterEvent.AddListener(PointerEnter);
        parentPiece.PointerExitEvent.AddListener(PointerExit);
        parentPiece.BeginDragEvent.AddListener(BeginDrag);
        parentPiece.EndDragEvent.AddListener(EndDrag);

        initialized = true;
    }

    void Update() {
        if (!initialized || parentPiece == null || isDeleting) return;
        SmoothFollow();
        FollowRotation();
        PieceTilt();
    }

    void SmoothFollow() {
        Vector3 targetWorldPos = ChessPiecesVisualManager.Instance.UIToWorldPosition(pieceTransform.position, fixedZ);
        targetWorldPos.z = parentPiece.isDragging ? fixedZ + dragZOffset : fixedZ;
        transform.position = Vector3.Lerp(transform.position, targetWorldPos, followSpeed * Time.deltaTime);
    }

    Vector3 UIToWorldPosition(Vector3 uiPosition) {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiPosition);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        worldPos.z = fixedZ;
        return worldPos;
    }

    void FollowRotation() {
        Vector3 movement = transform.position - pieceTransform.position;
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (parentPiece.isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    void PieceTilt() {
        float sine = parentPiece.isDragging ? 0 : Mathf.Sin(Time.time) * (parentPiece.isHovering ? .2f : 1);
        float cosine = parentPiece.isDragging ? 0 : Mathf.Cos(Time.time) * (parentPiece.isHovering ? .2f : 1);

        Vector3 mouseWorldPos = ChessPiecesVisualManager.Instance.MouseWorldPos;
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

    public void PlayDeleteAnimation(System.Action onComplete) {
        isDeleting = true;

        deleteSequence = DOTween.Sequence();

        deleteSequence
            .Append(transform.DOShakeScale(0.2f, 0.5f, 10, 90, false))
            .Append(transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(0f, 0.25f).SetEase(Ease.InBack))
            .OnComplete(() => {
                onComplete?.Invoke();
            });
    }

    public void ResetVisual() {
        if (deleteSequence != null && deleteSequence.IsActive())
            deleteSequence.Kill();

        transform.localScale = Vector3.one;
    }

    public void UpdateHoldVisual(float t) {
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.1f, t);
    }

    void BeginDrag(ChessPieces piece) {
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
    }

    void EndDrag(ChessPieces piece) {
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    void PointerEnter(ChessPieces piece) {
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    void PointerExit(ChessPieces piece) {
        if (!parentPiece.wasDragged)
            transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }
}
