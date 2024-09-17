using UnityEngine;
using UnityEngine.EventSystems;

public class UIBoardItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArrangementController ac; // I realize this is jank, will polish later

    public Texture2D normalCursor;
    public Texture2D grabbyCursor;

    public Vector2 grabbyOffset = Vector2.zero;

    public Vector2Int boardPos;

    private bool IsDragging;

    public void OnPointerDown(PointerEventData pointerEventData) {
        Debug.Assert(!IsDragging);
        if (!ac.BoardSlotHasItem(boardPos))
            return;

        Cursor.SetCursor(grabbyCursor, grabbyOffset, CursorMode.Auto);
        IsDragging = true;
    }
    
    public void OnPointerUp(PointerEventData pointerEventData) {
        if (!IsDragging)
            return;

        IsDragging = false;
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        Vector2 mouseCoords = Input.mousePosition;
        int invPos = ArrangementController.FindInventoryLocation(mouseCoords);
        bool success = false;
        if (invPos == -1) { // not in inventory, check board
            Vector2Int boardLoc = ArrangementController.FindTileLocation(mouseCoords);
            success = ac.UpdateBoardFromBoard(boardLoc, -1, boardPos);
        } else {
            success = ac.UpdateInvFromBoard(boardPos, invPos);
        }
    }
}
