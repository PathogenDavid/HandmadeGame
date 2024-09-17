using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrangementController : MonoBehaviour
{
    class DummyItem
    {
        public readonly Color Color;

        public DummyItem(Color color)
            => Color = color;
    }

    public GameObject inventory;
    public GameObject grid;

    private DummyItem[,] Board = new DummyItem[3,3];
    public List<GameObject> internalDisplay = new(); // BoardObjects
    public readonly int BoardWidth = 3;
    public readonly int BoardHeight = 3;
    
    private DummyItem[] Inventory = new DummyItem[9];
    public List<GameObject> hotbar = new List<GameObject>(9); // InventoryObjects

    private Color[] Colors;

    private Quest currentQuest;

    private void Awake()
    {
        for (int i = 0; i < hotbar.Count; i++)
            Inventory[i] = new(hotbar[i].GetComponent<Image>().color);

        SyncDisplay();
    }

    public void StartArrangementMode(Quest quest) {
        currentQuest = quest;
    }

    private void ShowInventory() {
        inventory.SetActive(true);
    }

    private void HideInventory() {
        inventory.SetActive(false);
    }

    private void ShowGrid() {
        grid.SetActive(true);
    }
    
    private void HideGrid() {
        grid.SetActive(false);
    }

    private void SyncDisplay()
    {
        Color none = new(0f, 0f, 0f, 0f);

        Debug.Assert(Inventory.Length == hotbar.Count);
        for (int i = 0; i < hotbar.Count; i++)
            hotbar[i].GetComponent<Image>().color = Inventory[i]?.Color ?? none;

        for (int r = 0; r < BoardHeight; r++)
        {
            for (int c = 0; c < BoardWidth; c++)
            {
                int internalDisplayIndex = c + (BoardHeight * r);
                internalDisplay[internalDisplayIndex].GetComponent<Image>().color = Board[r, c]?.Color ?? none;
            }
        }
    }

    public bool BoardSlotHasItem(Vector2Int pos)
        => Board[pos[0], pos[1]] != null;

    public bool InventorySlotHasItem(int slot)
        => Inventory[slot] != null;

    public bool UpdateBoardFromInv(Vector2Int pos, int invPos, Image img) {
        if (pos[0] == -1 || pos[1] == -1)
            return false; // invalid tile replacement
        DummyItem temp = Board[pos[0], pos[1]];
        Board[pos[0], pos[1]] = Inventory[invPos];
        Inventory[invPos] = temp;
        SyncDisplay();
        return true;
    }

    public bool UpdateBoardFromBoard(Vector2Int newPos, int invPos, Vector2Int curPos, Image img) {
        if (newPos[0] == -1 || newPos[1] == -1)
            return false; // invalid tile replacement
        DummyItem temp = Board[newPos[0], newPos[1]];
        Board[newPos[0], newPos[1]] = Board[curPos[0], curPos[1]];
        Board[curPos[0], curPos[1]] = temp;
        SyncDisplay();
        return true;
    }

    public bool UpdateInvFromBoard(Vector2Int curPos, int invPos, Image img) {
        DummyItem temp = Inventory[invPos];
        Inventory[invPos] = Board[curPos[0], curPos[1]];
        Board[curPos[0], curPos[1]] = temp;
        SyncDisplay();
        return true;
    }

    public bool UpdateInvFromInv(int curInvPos, int newInvPos, Image img) {
        DummyItem temp = Inventory[newInvPos];
        Inventory[newInvPos] = Inventory[curInvPos];
        Inventory[curInvPos] = temp;
        SyncDisplay();
        return true;
    }

    internal static int FindInventoryLocation(Vector2 mouseCoords) {
        int boardPos = -1;
        //Debug.Log(new Vector2(mouseCoords[0], Screen.width));
        // first, make sure on inventory
        if (mouseCoords[0] < .15 * Screen.width || mouseCoords[0] > .85 * Screen.width || mouseCoords[1] < .08 * Screen.height || mouseCoords[1] > .18 * Screen.height)
            return -1;
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

    internal static Vector2Int FindTileLocation(Vector2 mouseCoords) {
        float relativeX = mouseCoords[0];
        float relativeY = mouseCoords[1];
        Vector2Int boardPos = new(-1, -1);
        // first, make sure on board
        if (relativeX < Screen.width / 3 || relativeX > 2 * Screen.width / 3 || relativeY < .314 * Screen.height || relativeY > .87 * Screen.height) {
            return boardPos;
        }
        // find col
        if (relativeX < .45 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 0);
        } else if (relativeX < .55 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 1);
        } else if (relativeX < .65 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 2);
        } else {
            boardPos = new Vector2Int(boardPos[0], -1);
            Debug.Log("you broke it. good job.");
        }
        // find row
        if (relativeY < Screen.height / 2) {
            boardPos = new Vector2Int(2, boardPos[1]);
        } else if (relativeY < .68 * Screen.height) {
            boardPos = new Vector2Int(1, boardPos[1]);
        } else if (relativeY < .86 * Screen.height) {
            boardPos = new Vector2Int(0, boardPos[1]);
        } else {
            boardPos = new Vector2Int(-1, boardPos[1]);
            Debug.Log("wow. you still broke it. i feel attacked.");
        }
        return boardPos;
    }
}
