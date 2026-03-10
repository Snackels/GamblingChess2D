using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ChessPieces : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    protected Canvas canvas;
    protected Image imageComponent;
    protected Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

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

    private Vector3 originalPosition;

    protected void Awake() {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        originalPosition = transform.localPosition;

        Color c = imageComponent.color;
        c.a = 0f;
        imageComponent.color = c;

        if (chessPieceVisual != null)
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

    void ClampPosition() {
        Vector2 canvasSize = (canvas.transform as RectTransform).sizeDelta;
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x / 2, canvasSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y / 2, canvasSize.y / 2);
        transform.localPosition = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData) {
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
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;
        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) {
        EndDragEvent.Invoke(this);
        isDragging = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;
        transform.localPosition = originalPosition;
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
    }
}