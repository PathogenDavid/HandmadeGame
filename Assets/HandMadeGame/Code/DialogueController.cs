using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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

    public AudioClip ButtonClickSound;

    private string RemainingText;
    private bool ShowButtonsForFinalText;

    public InventoryHotBarController InventoryHotBar;

    public float SecondsPerCharacter = 0.01f;
    public float CharacterTimer = 0f;
    public int RemainingCharacterCount;
    private bool IsAnimating => RemainingCharacterCount > 0;

    private Random ChirpRandomness;
    public AudioClip[] ChirpFont { get; set; }

    private static Action NoOp = () => { };

    private void Awake()
        => gameObject.SetActive(false);

    private void Start()
    {
        YesButton.onClick.AddListener(() =>
        {
            SoundEffectsController.Instance.PlayUiSound(ButtonClickSound);
            HandleAction(YesAction);
        });

        NoButton.onClick.AddListener(() =>
        {
            SoundEffectsController.Instance.PlayUiSound(ButtonClickSound);
            HandleAction(NoAction);
        });
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

        // Initialize dialogue text animation
        DialogueText.ForceMeshUpdate(ignoreActiveState: true);
        DialogueText.maxVisibleCharacters = 0;
        CharacterTimer = SecondsPerCharacter;
        RemainingCharacterCount = DialogueText.textInfo.characterCount;

        ChirpRandomness = new Random(DialogueText.text.Length);

        // Buttons are not actually made visible until dialogue stops animating
        ButtonsAreVisible = showButtons;
        YesButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);

        _DialogueWasJustShown = true;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            InventoryHotBar.Hide();
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
            InventoryHotBar.Show();
            UiController.EndUiInteraction();
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (IsAnimating)
        {
            CharacterTimer -= Time.deltaTime;
            if (CharacterTimer < 0f)
            {
                CharacterTimer = SecondsPerCharacter;
                RemainingCharacterCount--;
                DialogueText.maxVisibleCharacters++;
                SoundEffectsController.Instance.PlayChirp(ChirpRandomness, ChirpFont);
            }

            // Allow skipping the animation
            if (UiController.CheckGlobalDismiss())
            {
                RemainingCharacterCount = 0;
                DialogueText.maxVisibleCharacters = int.MaxValue;
            }

            // Show buttons when animation ends if they were requested
            if (!IsAnimating && ButtonsAreVisible)
            {
                YesButton.gameObject.SetActive(true);
                NoButton.gameObject.SetActive(true);
            }
        }
        else if (!ButtonsAreVisible)
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
