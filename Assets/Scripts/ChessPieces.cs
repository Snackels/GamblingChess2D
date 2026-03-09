using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ChessPieces : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {

    Canvas canvas;
    Image imageComponent;

    [Header("Events")]
    [HideInInspector] public UnityEvent<ChessPieces> PointerEnterEvent;
    [HideInInspector] public UnityEvent<ChessPieces> PointerExitEvent;
    [HideInInspector] public UnityEvent<ChessPieces, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<ChessPieces> PointerDownEvent;
    [HideInInspector] public UnityEvent<ChessPieces> BeginDragEvent;
    [HideInInspector] public UnityEvent<ChessPieces> EndDragEvent;
    [HideInInspector] public UnityEvent<ChessPieces, bool> SelectEvent;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        BeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        EndDragEvent?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.localScale = Vector3.one * 1.1f;
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.localScale = Vector3.one;
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData) {
        PointerDownEvent?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData) {
        PointerUpEvent?.Invoke(this, true);
    }
}
