using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrangementController : MonoBehaviour
{
    public class DummyItem
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

    public Texture2D normalCursor;
    public Texture2D grabbyCursor;
    public Vector2 grabbyOffset = Vector2.zero;
    private UiItemSlot CurrentGrabTarget = null;

    private Quest currentQuest;

    private void Awake()
    {
        for (int i = 0; i < hotbar.Count; i++)
            Inventory[i] = new(hotbar[i].GetComponent<Image>().color);

        SyncDisplay();
    }

    void Start()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
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

    public ref DummyItem GetBoardItemSlot(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= BoardWidth || pos.y < 0 || pos.y >= BoardHeight)
            throw new ArgumentOutOfRangeException(nameof(pos));

        return ref Board[pos.x, pos.y];
    }

    public ref DummyItem GetInventoryItemSlot(int slot)
    {
        if (slot < 0 || slot >= Inventory.Length)
            throw new ArgumentOutOfRangeException(nameof(slot));

        return ref Inventory[slot];
    }

    /// <summary>Used to indicate that the specified slot wants to be grabbed</summary>
    public void StartGrab(UiItemSlot grabTarget)
    {
        // It shouldn't be possible for a grab to start while another is active
        Debug.Assert(CurrentGrabTarget == null);
        CurrentGrabTarget = null;

        // Ignore grabs from empty slots
        if (grabTarget.ItemSlot == null)
            return;

        CurrentGrabTarget = grabTarget;
        Cursor.SetCursor(grabbyCursor, grabbyOffset, CursorMode.Auto);
    }

    public void EndGrab(UiItemSlot grabTarget)
    {
        // Ignore grabs not from the current slot
        // (Generally means an empty slot is being grabbed)
        if (grabTarget != CurrentGrabTarget)
        {
            Debug.Assert(CurrentGrabTarget == null);
            return;
        }

        CurrentGrabTarget = null;
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        Vector2 mouseCoords = Input.mousePosition;

        ref DummyItem sourceSlot = ref grabTarget.ItemSlot;

        if (FindInventoryLocation(mouseCoords, out int invPos))
        {
            ref DummyItem destSlot = ref GetInventoryItemSlot(invPos);
            DummyItem temp = destSlot;
            destSlot = sourceSlot;
            sourceSlot = temp;
        }
        else if (FindTileLocation(mouseCoords, out Vector2Int boardPos))
        {
            ref DummyItem destSlot = ref GetBoardItemSlot(boardPos);
            DummyItem temp = destSlot;
            destSlot = sourceSlot;
            sourceSlot = temp;
        }

        SyncDisplay();
    }

    private void SyncDisplay()
    {
        Color none = new(0f, 0f, 0f, 0f);

        Debug.Assert(Inventory.Length == hotbar.Count);
        for (int i = 0; i < hotbar.Count; i++)
            hotbar[i].GetComponent<Image>().color = Inventory[i]?.Color ?? none;

        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                int internalDisplayIndex = x + (BoardHeight * y);
                internalDisplay[internalDisplayIndex].GetComponent<Image>().color = Board[x, y]?.Color ?? none;
            }
        }
    }

    internal static bool FindInventoryLocation(Vector2 mouseCoords, out int boardPos) {
        //Debug.Log(new Vector2(mouseCoords[0], Screen.width));
        // first, make sure on inventory
        if (mouseCoords[0] < .15 * Screen.width || mouseCoords[0] > .85 * Screen.width || mouseCoords[1] < .08 * Screen.height || mouseCoords[1] > .18 * Screen.height) {
            boardPos = -1;
            return false;
        }

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
            return false;
        }
        return true;
    }

    internal static bool FindTileLocation(Vector2 mouseCoords, out Vector2Int boardPos) {
        float relativeX = mouseCoords[0];
        float relativeY = mouseCoords[1];
        // first, make sure on board
        if (relativeX < Screen.width / 3 || relativeX > 2 * Screen.width / 3 || relativeY < .314 * Screen.height || relativeY > .87 * Screen.height) {
            boardPos = new Vector2Int(-1, -1);
            return false;
        }
        // find col
        int x;
        if (relativeX < .45 * Screen.width) {
            x = 0;
        } else if (relativeX < .55 * Screen.width) {
            x = 1;
        } else if (relativeX < .65 * Screen.width) {
            x = 2;
        } else {
            Debug.Log("you broke it. good job.");
            boardPos = new Vector2Int(-1, -1);
            return false;
        }
        // find row
        int y;
        if (relativeY < Screen.height / 2) {
            y = 2;
        } else if (relativeY < .68 * Screen.height) {
            y = 1;
        } else if (relativeY < .86 * Screen.height) {
            y = 0;
        } else {
            Debug.Log("wow. you still broke it. i feel attacked.");
            boardPos = new Vector2Int(-1, -1);
            return false;
        }

        Debug.Assert(x != -1 && y != -1);
        boardPos = new Vector2Int(x, y);
        return true;
    }
}
