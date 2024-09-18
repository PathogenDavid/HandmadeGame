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
    private UiItemSlot CurrentDropTarget = null;

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

    /// <summary>Used to indicate that the specified slot was dropped after a grab</summary>
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

        if (CurrentDropTarget != null)
        {
            ref DummyItem destSlot = ref CurrentDropTarget.ItemSlot;
            DummyItem temp = destSlot;
            destSlot = sourceSlot;
            sourceSlot = temp;
        }

        SyncDisplay();
    }

    public void HoverStart(UiItemSlot slot)
        => CurrentDropTarget = slot;

    public void HoverEnd(UiItemSlot slot)
    {
        if (CurrentDropTarget == slot)
            CurrentDropTarget = null;
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
}
