using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
        => CursorController.Instance.SetCursor(CursorKind.Point);

    public void OnPointerExit(PointerEventData eventData)
        => CursorController.Instance.SetCursor(CursorKind.Default);
}
