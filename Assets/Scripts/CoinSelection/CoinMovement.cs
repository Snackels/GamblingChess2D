using UnityEngine;
using DG.Tweening;

public class CoinMovement : MonoBehaviour {
    bool initialized = false;

    [Header("Chess Piece")]
    public CoinPiece parentPiece;
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

    public void Initialize(CoinPiece piece) {      // FIX: was Initialize(CoinMovement) — must accept CoinPiece
        parentPiece = piece;                       // FIX: was piece.parentPiece — piece IS the CoinPiece directly
        pieceTransform = piece.transform;
        canvas = piece.GetComponentInParent<Canvas>();
        fixedZ = transform.position.z;

        ChessPiecesVisualManager.Instance.SetCanvas(canvas);

        parentPiece.PointerEnterEvent.AddListener(PointerEnter);
        parentPiece.PointerExitEvent.AddListener(PointerExit);
        parentPiece.BeginDragEvent.AddListener(BeginDrag);
        parentPiece.EndDragEvent.AddListener(EndDrag);
        parentPiece.SelectedEvent.AddListener(Selected);
        initialized = true;
    }

    void OnDestroy() {
        if (parentPiece != null) {
            parentPiece.PointerEnterEvent.RemoveListener(PointerEnter);
            parentPiece.PointerExitEvent.RemoveListener(PointerExit);
            parentPiece.BeginDragEvent.RemoveListener(BeginDrag);
            parentPiece.EndDragEvent.RemoveListener(EndDrag);
            parentPiece.SelectedEvent.RemoveListener(Selected);
        }
        DOTween.Kill(transform);
        DOTween.Kill(2, true);
    }

    void Update() {
        if (!initialized || parentPiece == null) return;
        SmoothFollow();
        FollowRotation();
        PieceTilt();
    }

    void SmoothFollow() {
        
    }

    void FollowRotation() {
        
    }

    void PieceTilt() {
        
    }

    private void Selected(CoinPiece piece, bool state) {   // FIX: was ChessPieces
        if (scaleAnimations)
            transform.DOScale(state ? scaleOnSelect : scaleOnHover, scaleTransition).SetEase(scaleEase);
    }

    void BeginDrag(CoinPiece piece) {                      // FIX: was ChessPieces
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
    }

    void EndDrag(CoinPiece piece) {                        // FIX: was ChessPieces
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    void PointerEnter(CoinPiece piece) {                   // FIX: was ChessPieces
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    void PointerExit(CoinPiece piece) {                    // FIX: was ChessPieces
        
    }
}
