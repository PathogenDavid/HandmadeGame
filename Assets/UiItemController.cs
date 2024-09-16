using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArrangementController ac; // I realize this is jank, will polish later

    public Texture2D normalCursor;
    public Texture2D grabbyCursor;

    public Vector2 grabbyOffset = Vector2.zero;

    public int invIndex;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        Cursor.SetCursor(grabbyCursor, grabbyOffset, CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData pointerEventData) {
        Debug.Log("what the fajita");
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        Vector2 mouseCoords = Input.mousePosition;
        Vector2 boardLoc = FindTileLocation(mouseCoords);
        ac.UpdateBoard(boardLoc, invIndex, this.gameObject.GetComponent<Image>());
        this.gameObject.SetActive(false);
    }

    private Vector2 FindTileLocation(Vector2 mouseCoords) {
        Debug.Log("what the taco");
        float relativeX = mouseCoords[0];
        float relativeY = mouseCoords[1];
        Vector2 boardPos = new Vector2(-1, -1);
        // first, make sure on board
        if (relativeX < Screen.width / 3 || relativeX > 2 * Screen.width / 3 || relativeY < .314 * Screen.height || relativeY > .87 * Screen.height) {
            return boardPos;
        }
        // find col
        if (relativeX < .45 * Screen.width) {
            boardPos = new Vector2(boardPos[0], 0);
            Debug.Log("what is happening");
            Debug.Log(0);
        } else if (relativeX < .55 * Screen.width) {
            boardPos = new Vector2(boardPos[0], 1);
            Debug.Log(1);
        } else if (relativeX < .65 * Screen.width) {
            boardPos = new Vector2(boardPos[0], 2);
            Debug.Log(2);
        } else {
            boardPos = new Vector2(boardPos[0], -1);
            Debug.Log("you broke it. good job.");
        }
        // find row
        if (relativeY < Screen.height / 2) {
            boardPos = new Vector2(2, boardPos[1]);
        } else if (relativeY < .68 * Screen.height) {
            boardPos = new Vector2(1, boardPos[1]);
        } else if (relativeY < .86 * Screen.height) {
            boardPos = new Vector2(0, boardPos[1]);
        } else {
            boardPos = new Vector2(-1, boardPos[1]);
            Debug.Log("wow. you still broke it. i feel attacked.");
        }
        return boardPos;
    }
}
