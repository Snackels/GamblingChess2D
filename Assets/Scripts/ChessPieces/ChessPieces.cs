using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;

public class ChessPieces : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {
    protected Canvas canvas;
    protected Image imageComponent;
    protected Vector3 offset;

    [Header("Movement")]
    [SerializeField] float moveSpeedLimit = 50;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    [Header("Visual")]
    [HideInInspector] public ChessPieceVisual chessPieceVisual;

    [Header("Events")]
    [HideInInspector] public UnityEvent<ChessPieces> PointerEnterEvent;
    [HideInInspector] public UnityEvent<ChessPieces> PointerExitEvent;
    [HideInInspector] public UnityEvent<ChessPieces> BeginDragEvent;
    [HideInInspector] public UnityEvent<ChessPieces> EndDragEvent;

    [HideInInspector] public System.Action<ChessPieces> OnPieceSold;
    [HideInInspector] public System.Action<ChessPieces> OnPiecePlaced;

    Vector3 originalPosition;
    public Cell mCurrentCell = null;
    public Cell mTargetCell = null;
    protected RectTransform mRectTransform = null;
    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();
    List<Cell> mCurrentThreats = new List<Cell>();
    public List<Cell> GetCurrentThreats() => mCurrentThreats;
    public void SetCurrentThreats(List<Cell> threats) => mCurrentThreats = threats;

    bool isHolding = false;
    float holdTime = 0f;
    [SerializeField] float requiredHoldTime = 0.75f;

    protected void ShowCells() {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = true;
    }

    protected void ClearCells() {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = false;
        mHighlightedCells.Clear();
    }

    public virtual List<Cell> GetThreatenedCells() {
        return new List<Cell>();
    }

    public virtual void Place(Cell newCell) {
        if (mCurrentCell != null)
            mCurrentCell.mCurrentPiece = null;

        mCurrentCell = newCell;
        mCurrentCell.mCurrentPiece = this;

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, newCell.transform.position),
            canvas.worldCamera,
            out canvasPos
        );

        transform.localPosition = new Vector3(canvasPos.x, canvasPos.y, 0);
        originalPosition = transform.localPosition;
        gameObject.SetActive(true);

        OnPiecePlaced?.Invoke(this);

        ThreatManager.Instance.RecalculateAllThreats();
    }

    public void RecalculateThreats() {
        ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
        mCurrentThreats = GetThreatenedCells();
        ThreatManager.Instance.RegisterThreats(this, mCurrentThreats);
    }

    protected virtual void Move() {
        mCurrentCell.mCurrentPiece = null;
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;
        transform.localPosition = mCurrentCell.mRectTransform.anchoredPosition;
        originalPosition = transform.localPosition;
        mTargetCell = null;
    }

    protected virtual void Awake() {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        originalPosition = transform.localPosition;
        Color c = imageComponent.color;
        c.a = 0f;
        imageComponent.color = c;
    }

    protected void Start() {
        if (chessPieceVisual != null && !chessPieceVisual.IsInitialized)
            chessPieceVisual.Initialize(this);
    }

    void Update() {
        ClampPosition();
        if (isDragging) {
            Vector2 targetPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                canvas.worldCamera,
                out targetPosition
            );
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,
                targetPosition - (Vector2)offset,
                moveSpeedLimit * Time.deltaTime
            );
        }

        if (isHolding) {
            holdTime += Time.deltaTime;
            float t = holdTime / requiredHoldTime;

            if (chessPieceVisual != null)
                chessPieceVisual.UpdateHoldVisual(t);

            if (holdTime >= requiredHoldTime) {
                isHolding = false;
                if (mCurrentCell != null && chessPieceVisual != null)
                    chessPieceVisual.PlayDeleteAnimation(RemovePiece);
                else if (mCurrentCell != null)
                    RemovePiece();
            }
        }
    }

    public void RemovePiece() {
        if (mCurrentCell != null) {
            mCurrentCell.mCurrentPiece = null;
            mCurrentCell = null;
        }

        ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
        mCurrentThreats.Clear();
        ClearCells();
        ThreatManager.Instance.RecalculateAllThreats();

        OnPieceSold?.Invoke(this);

        if (chessPieceVisual != null)
            Destroy(chessPieceVisual.gameObject);

        Destroy(gameObject);
    }

    void CancelHold() {
        if (!isHolding) return;
        isHolding = false;
        holdTime = 0f;

        if (chessPieceVisual != null)
            chessPieceVisual.ResetVisual();
    }

    void ClampPosition() {
        Vector2 canvasSize = (canvas.transform as RectTransform).sizeDelta;
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x / 2, canvasSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y / 2, canvasSize.y / 2);
        transform.localPosition = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        CancelHold();

        if (mCurrentCell != null) {
            ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
            mCurrentThreats.Clear();
            mCurrentCell.mCurrentPiece = null;
            ThreatManager.Instance.RecalculateAllThreats();
        }

        BeginDragEvent.Invoke(this);
        Vector2 mouseCanvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out mouseCanvasPos
        );
        offset = mouseCanvasPos - (Vector2)transform.localPosition;
        isDragging = true;
        imageComponent.raycastTarget = false;
        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) {
        EndDragEvent.Invoke(this);
        isDragging = false;
        imageComponent.raycastTarget = true;

        if (mTargetCell != null)
            mTargetCell.mOutlineImage.enabled = false;

        if (mTargetCell != null)
            Place(mTargetCell);
        else {
            if (mCurrentCell != null)
                mCurrentCell.mCurrentPiece = this;
            transform.localPosition = originalPosition;
            ThreatManager.Instance.RecalculateAllThreats();
        }

        mTargetCell = null;
        ClearCells();

        StartCoroutine(FrameWait());
        IEnumerator FrameWait() {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        PointerExitEvent.Invoke(this);
        isHovering = false;
        CancelHold();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (mCurrentCell == null) return;
        isHolding = true;
        holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData) {
        CancelHold();
    }
}