using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;

public class ChessPieces : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {
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
    [HideInInspector] public System.Action<ChessPieces> OnSingleClick;
    [HideInInspector] public System.Action<ChessPieces> OnDoubleClick;
    [HideInInspector] public System.Action<ChessPieces> OnHoldComplete;

    [HideInInspector] public int turnsOnBoard = 0;
    [HideInInspector] public int spawnTurn = -1;
    [HideInInspector] public bool mustMove = false;
    [HideInInspector] public bool hasMovedThisTurn = false;

    Vector3 originalPosition;

    bool isHolding = false;
    float holdTime = 0f;
    [SerializeField] float requiredHoldTime = 0.75f;

    bool dragAllowed = false;
    bool wasUnplacedOnDragStart = false;

    public bool isActive = true;

    public bool isSelectedForUpkeep = false;

    public float baseCost = 0f;
    [HideInInspector] public float owedUpkeep = 0f;
    [HideInInspector] public float lockedUpkeep = -1f;
    protected bool _wasReactivatedThisTurn = false;

    float pointerDownTime;

    float _lastClickTime = -1f;
    [SerializeField] float doubleClickWindow = 0.3f;

    public void RemoveVisual() {
        if (chessPieceVisual != null) {
            DOTween.Kill(chessPieceVisual.transform);
            Destroy(chessPieceVisual.gameObject);
            chessPieceVisual = null;
        }
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
    }

    public void SyncOriginalPosition() {
        originalPosition = transform.localPosition;
    }

    void ClampPosition() {
        Vector2 canvasSize = (canvas.transform as RectTransform).sizeDelta;
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x / 2, canvasSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y / 2, canvasSize.y / 2);
        transform.localPosition = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        PointerExitEvent.Invoke(this);
        isHovering = false;

    }

    public void OnPointerDown(PointerEventData eventData) {

        isHolding = true;
        holdTime = 0f;
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData) {
        float pointerUpTime = Time.time;

        ChessPieceSFX sfx = GetComponent<ChessPieceSFX>();
        if (sfx != null) sfx.HandlePointerUp(pointerDownTime);
    }
}