using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBoardItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArrangementController ac; // I realize this is jank, will polish later

    public Texture2D normalCursor;
    public Texture2D grabbyCursor;

    public Vector2 grabbyOffset = Vector2.zero;

    public int invIndex;
    public Vector2 boardPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        Cursor.SetCursor(grabbyCursor, grabbyOffset, CursorMode.Auto);
    }
    
    public void OnPointerUp(PointerEventData pointerEventData) {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        Vector2 mouseCoords = Input.mousePosition;
        int invPos = FindInventoryLocation(mouseCoords);
        bool success = false;
        if (invPos == -1) { // not in inventory, check board
            Debug.Log("noooooo");
            Vector2 boardLoc = FindTileLocation(mouseCoords);
            success = ac.UpdateBoardFromBoard(boardLoc, -1, boardPos, this.gameObject.GetComponent<Image>());
        } else {
            success = ac.UpdateInvFromBoard(boardPos, invPos, this.gameObject.GetComponent<Image>());
        }
    }

    private int FindInventoryLocation(Vector2 mouseCoords) {
        int boardPos = -1;
        Debug.Log(new Vector2(mouseCoords[0], Screen.width));
        // first, make sure on inventory
        if (mouseCoords[0] < .15 * Screen.width || mouseCoords[0] > .85 * Screen.width || mouseCoords[1] < .08 * Screen.height || mouseCoords[1] > .18 * Screen.height) return -1;
        if (mouseCoords[0] < .22 * Screen.width) {
            boardPos = 0;
        } else if (mouseCoords[0] < .3 * Screen.width) {
            boardPos = 1;
        } else if (mouseCoords[0] < .38 * Screen.width) {
            boardPos = 2;
        } else if (mouseCoords[0] < .46 * Screen.width) {
            boardPos = 3;
        } else if (mouseCoords[0] < .54 * Screen.width) {
            boardPos = 4;
        } else if (mouseCoords[0] < .62 * Screen.width) {
            boardPos = 5;
        } else if (mouseCoords[0] < .7 * Screen.width) {
            boardPos = 6;
        } else if (mouseCoords[0] < .78 * Screen.width) {
            boardPos = 7;
        } else if (mouseCoords[0] < .85 * Screen.width) {
            boardPos = 8;
        } else {
            boardPos = -1;
            Debug.Log("stop breaking my code >:(");
        }
        return boardPos;
    }

    private Vector2 FindTileLocation(Vector2 mouseCoords) {
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
        } else if (relativeX < .55 * Screen.width) {
            boardPos = new Vector2(boardPos[0], 1);
        } else if (relativeX < .65 * Screen.width) {
            boardPos = new Vector2(boardPos[0], 2);
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
