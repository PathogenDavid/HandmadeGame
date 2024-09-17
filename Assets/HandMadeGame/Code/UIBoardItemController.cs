using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBoardItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArrangementController ac; // I realize this is jank, will polish later

    public Texture2D normalCursor;
    public Texture2D grabbyCursor;

    public Vector2 grabbyOffset = Vector2.zero;

    public Vector2Int boardPos;

    private void Awake()
        => gameObject.SetActive(false);

    public void OnPointerDown(PointerEventData pointerEventData) {
        Cursor.SetCursor(grabbyCursor, grabbyOffset, CursorMode.Auto);
    }
    
    public void OnPointerUp(PointerEventData pointerEventData) {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        Vector2 mouseCoords = Input.mousePosition;
        int invPos = ArrangementController.FindInventoryLocation(mouseCoords);
        bool success = false;
        if (invPos == -1) { // not in inventory, check board
            Vector2Int boardLoc = ArrangementController.FindTileLocation(mouseCoords);
            success = ac.UpdateBoardFromBoard(boardLoc, -1, boardPos, this.gameObject.GetComponent<Image>());
        } else {
            success = ac.UpdateInvFromBoard(boardPos, invPos, this.gameObject.GetComponent<Image>());
        }
    }
}
