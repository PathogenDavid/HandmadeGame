using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UiItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArrangementController ac;

    public abstract ref ArrangementController.DummyItem ItemSlot { get; }

    public void OnPointerDown(PointerEventData pointerEventData)
        => ac.StartGrab(this);

    public void OnPointerUp(PointerEventData pointerEventData)
        => ac.EndGrab(this);
}
