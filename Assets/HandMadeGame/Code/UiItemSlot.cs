using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UiItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ArrangementController Controller;

    public abstract ref NestItem ItemSlot { get; }

    public void OnPointerDown(PointerEventData pointerEventData)
        => Controller.StartGrab(this);

    public void OnPointerUp(PointerEventData pointerEventData)
        => Controller.EndGrab(this);

    public void OnPointerEnter(PointerEventData eventData)
        => Controller.HoverStart(this);

    public void OnPointerExit(PointerEventData eventData)
        => Controller.HoverEnd(this);
}
