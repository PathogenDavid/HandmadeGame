using System.Collections.Generic;
using UnityEngine;

public class HudTests : MonoBehaviour
{
    public DialogueController DialogueController;
    public ControllerPrompt ControllerPrompt;
    public InfoPopupController InfoPopupController;

    public Sprite Snel;
    public Sprite Snel2;

    private bool UiActive;

    private Quest[] AllQuests;
    private string CurrentRamble = null;
    private Sprite RamblePortrait;
    private Queue<(string label, string dialogue)> Rambles = new();

    private void Awake()
    {
        UiController.UiInteractionStart += () => UiActive = true;
        UiController.UiInteractionEnd += () => UiActive = false;

        AllQuests = GameObject.FindObjectsOfType<Quest>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 0f, 120f, 1000f));
        GUILayout.Label($"UI is {(UiActive ? "active" : "inactive")} (depth {UiController.UiSessionDepth})");

        if (GUILayout.Button("Basic Test"))
        {
            DialogueController.ShowDialogue(Snel, "Test");
        }

        if (GUILayout.Button("Conversation Test"))
        {
            DialogueController.ShowDialogue(Snel, "Test?###Two", () => DialogueController.ShowDialogue(Snel2, "Test!"));
        }

        if (GUILayout.Button("Choice"))
        {
            DialogueController.ShowDialogue(Snel, "Test?\n###\nTwo",
                () => DialogueController.ShowDialogue(Snel2, "Yes"),
                () => DialogueController.ShowDialogue(Snel2, "No")
            );
        }

        if (GUILayout.Button("Custom choice"))
        {
            DialogueController.ShowDialogue(Snel, "Test?###Two",
                "Yes, I'm ready!", () => { },
                "Not quite...", () => { }
            );
        }

        if (GUILayout.Button("Show prompt E"))
            ControllerPrompt.Show(KeyCode.E, "Do the thing");
        if (GUILayout.Button("Show prompt F"))
            ControllerPrompt.Show(KeyCode.F, "Do the other thing");
        if (GUILayout.Button("Show prompt G"))
            ControllerPrompt.Show(KeyCode.G, "Do the other thing");
        if (GUILayout.Button("Hide prompt"))
            ControllerPrompt.Hide();

        if (GUILayout.Button("Info Popup"))
            InfoPopupController.ShowPopup("This is a test of the emergency birdcast system, don't let it get your feathers ruffled!", () => Debug.Log("Test popup dismissed"));

        GUILayout.Label("=== Ramble ===");
        foreach (Quest quest in AllQuests)
        {
            if (GUILayout.Button(quest.name))
            {
                Rambles.Clear();
                RamblePortrait = quest.CharacterPortrait;
                Rambles.Enqueue(("StartDialogue", quest.StartDialogue));
                Rambles.Enqueue(("QuestAcceptedDialogue", quest.QuestAcceptedDialogue));
                Rambles.Enqueue(("QuestDescription", quest.QuestDescription));
                Rambles.Enqueue(("QuestRejectedDialogue", quest.QuestRejectedDialogue));
                Rambles.Enqueue(("ReputationRequirementNotMetDialogue", quest.ReputationRequirementNotMetDialogue));
                Rambles.Enqueue(("QuestAlreadyCompletedDialogue", quest.QuestAlreadyCompletedDialogue));
                Rambles.Enqueue(("ReadyToBeginDialogue", quest.ReadyToBeginDialogue));
                Rambles.Enqueue(("NotReadyToBeginDialogue", quest.NotReadyToBeginDialogue));
                Rambles.Enqueue(("NoNeedToRepeatDialogue", quest.NoNeedToRepeatDialogue));
                Rambles.Enqueue(("QuestCompleteDialogue", quest.QuestCompleteDialogue));
                Rambles.Enqueue(("WrongItemsDialogue", quest.WrongItemsDialogue));
                Rambles.Enqueue(("NotEnoughItemsDialogue", quest.NotEnoughItemsDialogue));
                Rambles.Enqueue(("WrongItemArrangementDialogue", quest.WrongItemArrangementDialogue));

                void ShowNextRamble()
                {
                    CurrentRamble = null;

                    if (Rambles.Count == 0)
                        return;

                    (string label, string dialogue) = Rambles.Dequeue();
                    CurrentRamble = label;
                    DialogueController.ShowDialogue(RamblePortrait, dialogue, () => ShowNextRamble());
                }

                ShowNextRamble();
            }
        }

        if (CurrentRamble != null)
        {
            GUILayout.Label($"Currently showing {CurrentRamble}");
            if (GUILayout.Button("Stop rambling"))
                Rambles.Clear();
        }

        GUILayout.EndArea();
    }
}
