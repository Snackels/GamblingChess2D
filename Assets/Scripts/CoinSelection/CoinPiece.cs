using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CoinPiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {
    protected Canvas canvas;
    protected Image imageComponent;
    protected Vector3 offset;

    [Header("Movement")]
    [SerializeField] float moveSpeedLimit = 30;

    [Header("States")]
    public bool isHovering;

    [Header("Visual")]
    [HideInInspector] public CoinMovement coinMovement;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CoinPiece> PointerEnterEvent;
    [HideInInspector] public UnityEvent<CoinPiece> PointerExitEvent;
    [HideInInspector] public UnityEvent<CoinPiece> BeginDragEvent;
    [HideInInspector] public UnityEvent<CoinPiece> EndDragEvent;
    [HideInInspector] public UnityEvent<CoinPiece, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CoinPiece> PointerDownEvent;
    [HideInInspector] public UnityEvent<CoinPiece, bool> SelectedEvent;

    [Header("Selection")]
    public bool isSelected;
    public float selectionOffset = 50f;

    protected Vector3 originalPosition;

    protected void Awake() 
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        originalPosition = transform.localPosition;

        Color c = imageComponent.color;
        c.a = 0f;
        imageComponent.color = c;
    }

    protected void Start() 
    {
        if (coinMovement != null)
            coinMovement.Initialize(this);         // FIX: was incorrectly invoking PointerEnterEvent instead of initializing
    }

    void Update() {
        ClampPosition();
    }

    void ClampPosition() {
        Vector2 canvasSize = (canvas.transform as RectTransform).sizeDelta;
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x / 2, canvasSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y / 2, canvasSize.y / 2);
        transform.localPosition = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
    }

    public void OnDrag(PointerEventData eventData) 
    {   
    }

    public void OnEndDrag(PointerEventData eventData) {
        
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        PointerDownEvent.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        isSelected = !isSelected;                  // FIX: was missing the toggle
        SelectedEvent.Invoke(this, isSelected);    // FIX: was missing the event invoke

        if (isSelected)
            transform.localPosition = originalPosition + Vector3.up * selectionOffset;
        else
            transform.localPosition = originalPosition;
    }

    public void Deselect() 
    {
        if (isSelected) {
            isSelected = false;
            SelectedEvent.Invoke(this, false);
            transform.localPosition = originalPosition;
        }
    }
}