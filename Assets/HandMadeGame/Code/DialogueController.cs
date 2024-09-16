using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class DialogueController : MonoBehaviour
{
    public Image Portrait;
    public TMP_Text DialogueText;
    public Button YesButton;
    public Button NoButton;
    public TMP_Text YesText;
    public TMP_Text NoText;
    private Action YesAction;
    private Action NoAction;
    private Action AdvanceAction;
    private bool ButtonsAreVisible = false;

    private string RemainingText;
    private bool ShowButtonsForFinalText;

    private static Action NoOp = () => { };

    private void Awake()
        => gameObject.SetActive(false);

    private void Start()
    {
        YesButton.onClick.AddListener(() => HandleAction(YesAction));
        NoButton.onClick.AddListener(() => HandleAction(NoAction));
    }

    private bool _DialogueWasJustShown = false;
    private void ShowDialogue(Sprite portrait, string message, bool showButtons)
    {
        // Check if the message contains a split. If it does then this will be an interim dialogue box with the remainder deferred
        const string messageSplit = "###";
        int messageSplitIndex = message.IndexOf(messageSplit);
        if (messageSplitIndex >= 0)
        {
            Debug.Assert(RemainingText == null);
            RemainingText = message.Substring(messageSplitIndex + messageSplit.Length);
            message = message.Substring(0, messageSplitIndex);
            ShowButtonsForFinalText = showButtons;
            showButtons = false;
        }

        Portrait.sprite = portrait;
        DialogueText.text = UiController.ProcessDisplayString(message);

        ButtonsAreVisible = showButtons;
        YesButton.gameObject.SetActive(showButtons);
        NoButton.gameObject.SetActive(showButtons);

        _DialogueWasJustShown = true;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            UiController.StartUiInteraction();
        }
    }

    public void ShowDialogue(Sprite portrait, string message, Action advanceAction)
    {
        YesAction = NoAction = null;
        AdvanceAction = advanceAction;
        ShowDialogue(portrait, message, false);
    }

    public void ShowDialogue(Sprite portrait, string message)
        => ShowDialogue(portrait, message, NoOp);

    public void ShowDialogue(Sprite portrait, string message, string yesText, Action yesAction, string noText, Action noAction)
    {
        YesText.text = yesText;
        NoText.text = noText;

        YesAction = yesAction;
        NoAction = noAction;
        AdvanceAction = null;
        ShowDialogue(portrait, message, true);
    }

    public void ShowDialogue(Sprite portrait, string message, Action yesAction, Action noAction)
        => ShowDialogue(portrait, message, "Yes", yesAction, "No", noAction);

    private void HandleAction(Action action)
    {
        _DialogueWasJustShown = false;
        action();

        // If the action didn't continue the dialogue, the dialogue session ends.
        if (!_DialogueWasJustShown)
        {
            gameObject.SetActive(false);
            UiController.EndUiInteraction();
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && !ButtonsAreVisible)
        {
            if (UiController.CheckGlobalDismiss())
            {
                if (RemainingText == null)
                {
                    HandleAction(AdvanceAction);
                }
                else
                {
                    string text = RemainingText;
                    RemainingText = null;
                    ShowDialogue(Portrait.sprite, text, ShowButtonsForFinalText);
                }
            }
        }
    }
}
