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

    [Header("References")]
    [SerializeField] Image mustMoveIcon;

    [Header("Movement")]
    [SerializeField] float moveSpeedLimit = 50;

    [SerializeField] float selectedYOffset = 30f;
    [SerializeField] float selectionTransition = 0.15f;

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

    [HideInInspector] public int turnsOnBoard = 0;
    [HideInInspector] public int spawnTurn = -1;
    [HideInInspector] public bool mustMove = false;
    [HideInInspector] public bool hasMovedThisTurn = false;

    Vector3 originalPosition;
    Cell cellAtTurnStart = null;
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

    bool dragAllowed = false;
    bool wasUnplacedOnDragStart = false;

    public bool isActive = true;
    public bool isSelectedForUpkeep = false;
    public float baseCost = 0f;
    [HideInInspector] public float owedUpkeep = 0f;  // unpaid upkeep from last end-turn
    protected bool _wasReactivatedThisTurn = false;

    float pointerDownTime;

    protected void ShowCells() {
        foreach (Cell cell in mHighlightedCells) {
            cell.isHighlighted = true;
            cell.mOutlineImage.enabled = true;
        }
    }

    protected void ClearCells() {
        foreach (Cell cell in mHighlightedCells) {
            cell.isHighlighted = false;
            cell.mOutlineImage.enabled = false;
        }
        mHighlightedCells.Clear();
    }

    public virtual List<Cell> GetThreatenedCells() {
        return new List<Cell>();
    }

    public virtual bool IsValidPlacement(Cell cell) {
        return true;
    }

    public virtual List<Cell> GetValidMoveCells() {
        return GetThreatenedCells();
    }

    public virtual void Place(Cell newCell) {

        Cell previousCell = mCurrentCell;
        if (mCurrentCell == null && spawnTurn == -1) {
            if (!ScoreManager.Instance.CanAfford(baseCost)) {
                transform.localPosition = originalPosition;
                return;
            }
            ScoreManager.Instance.SpendMoney(baseCost);
        }

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

        if (spawnTurn == -1)
            spawnTurn = TurnManager.Instance.currentTurn;

        OnPiecePlaced?.Invoke(this);
        GameManager.Instance.NotifyBoardChanged();
        ThreatManager.Instance.RecalculateAllThreats();
        _wasReactivatedThisTurn = false;
        if (mustMove && previousCell != null && newCell != cellAtTurnStart) {
            SetMustMove(false);
            hasMovedThisTurn = true;
        }
    }

    public void RemovePiece() {

        ScoreManager.Instance.AddMoney(GetRefundValue());

        if (mCurrentCell != null) {
            mCurrentCell.mCurrentPiece = null;
            mCurrentCell = null;
        }

        mustMove = false;
        hasMovedThisTurn = false;
        cellAtTurnStart = null;
        ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
        mCurrentThreats.Clear();
        ClearCells();
        ThreatManager.Instance.RecalculateAllThreats();

        OnPieceSold?.Invoke(this);
        GameManager.Instance.NotifyBoardChanged();

        if (chessPieceVisual != null) {
            DOTween.Kill(chessPieceVisual.transform);
            Destroy(chessPieceVisual.gameObject);
        }

        Destroy(gameObject);
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

    void CancelHold() {
        if (!isHolding) return;
        isHolding = false;
        holdTime = 0f;

        if (chessPieceVisual != null)
            chessPieceVisual.ResetVisual();
    }

    public void SetMustMove(bool value) {
        mustMove = value;
        if (mustMove) {
            cellAtTurnStart = mCurrentCell;
            hasMovedThisTurn = false;
        }
        if (mustMoveIcon != null)
            mustMoveIcon.enabled = mustMove;
    }

    public virtual void Penalty() {
        // Blank for now — will be implemented later.
    }

    public void ToggleUpkeepSelection() {
        if (mCurrentCell == null) return;

        // If piece is inactive and has owed upkeep, let player pay now to reactivate
        if (!isActive) {
            TryPayOwedUpkeep();
            return;
        }

        isSelectedForUpkeep = !isSelectedForUpkeep;
        float targetY = isSelectedForUpkeep
            ? originalPosition.y + selectedYOffset
            : originalPosition.y;
        transform.DOLocalMoveY(targetY, selectionTransition).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Called mid-turn when a player clicks an inactive piece to pay its owed upkeep.
    /// </summary>
    public void TryPayOwedUpkeep() {
        if (isActive || owedUpkeep <= 0f) return;
        if (!ScoreManager.Instance.CanAfford(owedUpkeep)) {
            Debug.Log($"[ChessPieces] Cannot afford upkeep {owedUpkeep} for {name}");
            return;
        }
        ScoreManager.Instance.SpendMoney(owedUpkeep);
        owedUpkeep = 0f;
        mustMove = false;
        hasMovedThisTurn = false;
        cellAtTurnStart = null;
        SetMustMove(false);
        _wasReactivatedThisTurn = true;
        SetActive();
        // Mark as selected so end-of-next-turn upkeep is expected
        isSelectedForUpkeep = true;
        float targetY = originalPosition.y + selectedYOffset;
        transform.DOLocalMoveY(targetY, selectionTransition).SetEase(Ease.OutBack);
        Debug.Log($"[ChessPieces] {name} reactivated via mid-turn upkeep payment");
    }

    public void ClearUpkeepSelection() {
        isSelectedForUpkeep = false;
        transform.DOLocalMoveY(originalPosition.y, selectionTransition).SetEase(Ease.OutSine);
    }

    public void SetInactive() {
        isActive = false;
        ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
        mCurrentThreats.Clear();
        Color c = imageComponent.color;
        c.a = 0.4f;
        imageComponent.color = c;
    }

    public void SetActive() {
        isActive = true;
        Color c = imageComponent.color;
        c.a = 0f;
        imageComponent.color = c;
        RecalculateThreats();
    }

    public float GetUpkeepCost() {
        return baseCost * (0.5f + 0.25f * turnsOnBoard);
    }

    public float GetRefundValue() {
        if (spawnTurn == TurnManager.Instance.currentTurn)
            return baseCost;
        return baseCost * 0.5f;
    }

    void ClampPosition() {
        Vector2 canvasSize = (canvas.transform as RectTransform).sizeDelta;
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x / 2, canvasSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y / 2, canvasSize.y / 2);
        transform.localPosition = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    bool CanDrag() {
        if (!isActive) return false;
        if (mCurrentCell == null) return true;
        if (hasMovedThisTurn) return false;
        if (mustMove) return true;
        if (spawnTurn == TurnManager.Instance.currentTurn) return true;
        if (_wasReactivatedThisTurn) return true;
        return false;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        CancelHold();
        dragAllowed = false;
        if (!CanDrag()) return;
        dragAllowed = true;

        wasUnplacedOnDragStart = (mCurrentCell == null);

        if (mCurrentCell != null) {
            ThreatManager.Instance.UnregisterThreats(this, mCurrentThreats);
            mCurrentThreats.Clear();
            mCurrentCell.mCurrentPiece = null;
            ThreatManager.Instance.RecalculateAllThreats();
        }

        if (mustMove || _wasReactivatedThisTurn) {
            mHighlightedCells = GetValidMoveCells();
            ShowCells();
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
        if (!dragAllowed) return;
        EndDragEvent.Invoke(this);
        isDragging = false;
        imageComponent.raycastTarget = true;

        if (mTargetCell != null)
            mTargetCell.mOutlineImage.enabled = false;

        bool isMovingOnBoard = (cellAtTurnStart != null && mustMove) || _wasReactivatedThisTurn;
        if (isMovingOnBoard && mTargetCell != null && !mHighlightedCells.Contains(mTargetCell))
            mTargetCell = null;

        if (mTargetCell != null && !IsValidPlacement(mTargetCell))
            mTargetCell = null;

        if (wasUnplacedOnDragStart && mTargetCell != null && GameManager.Instance.IsBoardFull())
            mTargetCell = null;

        if (mTargetCell != null) { Place(mTargetCell); }
        else {
            if (cellAtTurnStart != null && mustMove) {
                cellAtTurnStart.mCurrentPiece = this;
                mCurrentCell = cellAtTurnStart;
            }
            else if (mCurrentCell != null) mCurrentCell.mCurrentPiece = this;
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

    float _lastClickTime = -1f;
    [SerializeField] float doubleClickWindow = 0.3f;

    public void OnPointerDown(PointerEventData eventData) {
        if (mCurrentCell == null) return;
        isHolding = true;
        holdTime = 0f;
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData) {
        float pointerUpTime = Time.time;
        CancelHold();

        if (pointerUpTime - pointerDownTime < 0.2f && !wasDragged && mCurrentCell != null) {
            // Check for double-click
            if (pointerUpTime - _lastClickTime <= doubleClickWindow) {
                _lastClickTime = -1f;
                ToggleUpkeepSelection();
            }
            else {
                _lastClickTime = pointerUpTime;
            }
        }
    }
}