using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameFlow : MonoBehaviour
{
    private static GameFlow _Instance;
    public static GameFlow Instance
    {
        get
        {
            if (_Instance is not null)
                return _Instance;

            return _Instance = FindObjectOfType<GameFlow>();
        }
    }

    [NonSerialized] public List<NestItem> AllNestItems = new();

    public const int InventorySize = 9;
    [NonSerialized] public NestItem[] Inventory = new NestItem[InventorySize];

    public int Reputation;
    public int WinReputation = 3;

    public DialogueController DialogueController;
    public InfoPopupController InfoPopupController;
    public ArrangementModeControllerBase ArrangementModeController;

    private Quest CurrentQuest;

    public bool ShowDebugger;

    private void Awake()
    {
        if (Instance != this)
        {
            Debug.LogError($"Multiple {nameof(GameFlow)} instances exist in the scene!");
            Destroy(this);
            return;
        }

        // Always go back to the exploration music when UI interactions end
        UiController.UiInteractionEnd += () => BackgroundMusicController.Instance.TransitionToExplorationMusic();
    }

    public void HandleInteraction(Quest quest)
    {
        BackgroundMusicController.Instance.TransitionToCharacterMusic(quest.CharacterMusic);
        DialogueController.ChirpFont = quest.CharacterTalkSounds;

        switch (quest.CurrentState)
        {
            case QuestState.NotStarted:
            {
                // Player does not have enough reputation
                if (Reputation < quest.RequiredReputation)
                {
                    DialogueController.ShowDialogue(quest.CharacterPortrait, quest.ReputationRequirementNotMetDialogue);
                    return;
                }

                if (CurrentQuest != null)
                {
                    // We don't plan for this to be possible, complain if it happens
                    Debug.LogError("Trying to handle game flow for a quest which has not been started when the current quest is still in progress!");
                    DialogueController.ShowDialogue(quest.CharacterPortrait, "Looks like you're already busy with another client. Come back to me when you're done with them!");
                    return;
                }

                DialogueController.ShowDialogue
                (
                    quest.CharacterPortrait,
                    quest.StartDialogue,
                    // Yes
                    () => DialogueController.ShowDialogue
                    (
                        quest.CharacterPortrait,
                        quest.QuestAcceptedDialogue,
                        () =>
                        {
                            InfoPopupController.ShowPopup(quest.QuestDescription);
                            AcceptQuest(quest);
                        }),
                    () => DialogueController.ShowDialogue(quest.CharacterPortrait, quest.QuestRejectedDialogue)
                );
                break;
            }
            case QuestState.Accepted:
                Debug.Assert(quest == CurrentQuest);

                DialogueController.ShowDialogue
                (
                    quest.CharacterPortrait,
                    quest.ReadyToBeginDialogue,
                    "Yes, I'm ready!",
                    () => ArrangementModeController.StartArrangementMode(quest),
                    "Not quite...",
                    () => DialogueController.ShowDialogue
                    (
                        quest.CharacterPortrait,
                        quest.NotReadyToBeginDialogue,
                        () => InfoPopupController.ShowPopup(quest.QuestDescription),
                        () => DialogueController.ShowDialogue(quest.CharacterPortrait, quest.NoNeedToRepeatDialogue)
                    )
                );
                break;
            case QuestState.Completed:
                DialogueController.ShowDialogue(quest.CharacterPortrait, quest.QuestAlreadyCompletedDialogue);
                break;
            default:
                Debug.LogError("!!! Quest state is invalid !!!");
                DialogueController.ShowDialogue(quest.CharacterPortrait, "Sorry friend, but my quest is corrupted!");
                break;
        }
    }

    private void AcceptQuest(Quest quest)
    {
        Debug.Assert(CurrentQuest == null);

        quest.CurrentState = QuestState.Accepted;
        CurrentQuest = quest;

        foreach (GameObject gameObject in quest.SpawnOnAccept)
            gameObject.SetActive(true);
    }

    public void HandleInteraction(NestItem item)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                Inventory[i] = item;
                item.gameObject.SetActive(false);
                return;
            }
        }

        Debug.LogWarning($"Can't pick up '{item}', inventory is full!");
        //TODO: Tell the player that the inventory is full
    }

    public void EndArrangementMode(Quest quest)
    {
        if (quest != CurrentQuest)
            throw new InvalidOperationException("Arrangement mode should not be ended with a quest that isn't the current quest!");

        BackgroundMusicController.Instance.TransitionToCharacterMusic();
        CursorController.Instance.SetCursor(CursorKind.Default);

        PuzzleOutcome outcome = quest.Validator.CheckPuzzle(quest);
        switch (outcome)
        {
            case PuzzleOutcome.Perfect: DialogueController.ShowDialogue(quest.CharacterPortrait, quest.QuestCompleteDialogue, () => MarkQuestCompleted(quest)); break;
            case PuzzleOutcome.WrongItemsPresent: DialogueController.ShowDialogue(quest.CharacterPortrait, quest.WrongItemsDialogue); break;
            case PuzzleOutcome.NotEnoughItems: DialogueController.ShowDialogue(quest.CharacterPortrait, quest.NotEnoughItemsDialogue); break;
            case PuzzleOutcome.IncorrectPlacement: DialogueController.ShowDialogue(quest.CharacterPortrait, quest.WrongItemArrangementDialogue); break;
            default:
                Debug.LogError($"Invalid puzzle outcome '{outcome}'!");
                DialogueController.ShowDialogue(quest.CharacterPortrait, "Your decorating was so good it broke the matrix!");
                break;
        }
    }

    private void MarkQuestCompleted(Quest quest)
    {
        Debug.Assert(quest == CurrentQuest);
        Reputation++;
        quest.CurrentState = QuestState.Completed;
        CurrentQuest = null;
        WinConditionCheck();
    }

    private void WinConditionCheck()
    {
        if (Reputation >= WinReputation)
            InfoPopupController.ShowPopup
            (
                "You successfully redecorated everyone's nest!\n" +
                "Now you're widely recognized in your park as an\n\n" +
                "<size=130%><<em>Expert Interior Birdecorator</em>></size>\n\n" +
                "Thanks for playing!"
            );
    }

    private void OnGUI()
    {
        if (!ShowDebugger)
            return;

        GUILayout.BeginArea(new Rect(140f, 0f, 120f, 1000f));
        GUILayout.Label($"Reputation: {Reputation}");
        GUILayout.Label($"Current quest: {(CurrentQuest == null ? "<none>" : CurrentQuest.name)}");

        if (GUILayout.Button("Rep+"))
        {
            Reputation++;
            WinConditionCheck();
        }

        if (CurrentQuest != null)
        {
            if (GUILayout.Button("Abort quest"))
            {
                CurrentQuest.CurrentState = QuestState.NotStarted;
                CurrentQuest = null;
            }

            if (GUILayout.Button("Complete quest"))
                MarkQuestCompleted(CurrentQuest);
        }
        GUILayout.EndArea();
    }
}
