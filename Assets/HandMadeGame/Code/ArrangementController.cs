using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class ArrangementController : ArrangementModeControllerBase
{
    private GameFlow GameFlow => GameFlow.Instance;

    public Camera MainCamera;
    public Camera ArrangementModeCamera;

    public ControllerPrompt ControllerPrompt;

    public GameObject GridVisuals;

    public UiItemSlot[] ItemSlots;

    private UiItemSlot CurrentGrabTarget = null;
    private UiItemSlot CurrentDropTarget = null;

    private Quest CurrentQuest;
    public bool IsActive => CurrentQuest != null;

    public AudioClip PickUpSound;
    public AudioClip PlaceDownSound;

    private void Awake()
    {
        foreach (UiItemSlot itemSlot in ItemSlots)
        {
            itemSlot.Controller = this;
            itemSlot.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }

        GridVisuals.SetActive(false);
        ArrangementModeCamera.gameObject.SetActive(false);
    }

    public override void StartArrangementMode(Quest quest)
    {
        Debug.Assert(CurrentQuest == null);
        CurrentQuest = quest;
        UiController.StartUiInteraction();
        GridVisuals.SetActive(true);
        ControllerPrompt.Show(UiController.InteractionKey, "Finish");
        MainCamera.gameObject.SetActive(false);
        ArrangementModeCamera.gameObject.SetActive(true);
        BackgroundMusicController.Instance.TransitionToDecoratingMusic();
    }

    private void EndArrangementMode()
    {
        Debug.Assert(CurrentQuest != null);
        ControllerPrompt.Hide();
        CurrentGrabTarget = null;
        CurrentDropTarget = null;
        GridVisuals.SetActive(false);
        UiController.EndUiInteraction();
        ArrangementModeCamera.gameObject.SetActive(false);
        MainCamera.gameObject.SetActive(true);
        GameFlow.EndArrangementMode(CurrentQuest);
        CurrentQuest = null;
    }

    private void Update()
    {
        if (CurrentQuest != null && UiController.CheckGlobalDismiss(clickToDismiss: false))
            EndArrangementMode();
    }

    public ref NestItem GetBoardItemSlot(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= CurrentQuest.BoardWidth || pos.y < 0 || pos.y >= CurrentQuest.BoardHeight)
            throw new ArgumentOutOfRangeException(nameof(pos));

        return ref CurrentQuest.Board[pos.x, pos.y];
    }

    public ref NestItem GetInventoryItemSlot(int slot)
    {
        if (slot < 0 || slot >= GameFlow.Inventory.Length)
            throw new ArgumentOutOfRangeException(nameof(slot));

        return ref GameFlow.Inventory[slot];
    }

    /// <summary>Used to indicate that the specified slot wants to be grabbed</summary>
    public void StartGrab(UiItemSlot grabTarget)
    {
        if (CurrentQuest == null)
        {
            Debug.LogWarning("Tried to start grab when arrangement mode is inactive!");
            return;
        }

        // It shouldn't be possible for a grab to start while another is active
        Debug.Assert(CurrentGrabTarget == null);
        CurrentGrabTarget = null;

        // Ignore grabs from empty slots
        if (grabTarget.ItemSlot == null)
            return;

        CurrentGrabTarget = grabTarget;
        CursorController.Instance.SetCursor(CursorKind.GrabbyHand);
        SoundEffectsController.Instance.PlayUiSound(PickUpSound);
    }

    /// <summary>Used to indicate that the specified slot was dropped after a grab</summary>
    public void EndGrab(UiItemSlot grabTarget)
    {
        if (CurrentQuest == null)
        {
            Debug.LogWarning("Tried to end grab when arrangement mode is inactive!");
            return;
        }

        // Ignore grabs not from the current slot
        // (Generally means an empty slot is being grabbed)
        if (grabTarget != CurrentGrabTarget)
        {
            Debug.Assert(CurrentGrabTarget == null);
            return;
        }

        CurrentGrabTarget = null;
        CursorController.Instance.SetCursor(CursorKind.OpenHand);
        Vector2 mouseCoords = Input.mousePosition;

        ref NestItem sourceSlot = ref grabTarget.ItemSlot;

        if (CurrentDropTarget != null)
        {
            ref NestItem destSlot = ref CurrentDropTarget.ItemSlot;
            NestItem temp = destSlot;
            destSlot = sourceSlot;
            sourceSlot = temp;
            SoundEffectsController.Instance.PlayUiSound(CurrentDropTarget is UiInvItemController ? PlaceDownSound : PickUpSound);
        }
    }

    public void HoverStart(UiItemSlot slot)
    {
        if (CurrentGrabTarget == null && slot.ItemSlot != null)
            CursorController.Instance.SetCursor(CursorKind.OpenHand);

        CurrentDropTarget = slot;
    }

    public void HoverEnd(UiItemSlot slot)
    {
        if (CurrentGrabTarget == null)
            CursorController.Instance.SetCursor(CursorKind.Default);

        if (CurrentDropTarget == slot)
            CurrentDropTarget = null;
    }
}
