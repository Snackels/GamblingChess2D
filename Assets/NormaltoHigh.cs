using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Highlight");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger("NormalState");
    }
}