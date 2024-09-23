using System;
using TMPro;
using UnityEngine;
using Random = System.Random;

public sealed class Prologue : MonoBehaviour
{
    public AudioClip[] ChirpFont;
    private Random ChirpRandomness = new(3226);

    public TMP_Text DialogueText;
    public GameObject ContinuePrompt;

    public float SecondsPerCharacter = 0.02f;
    public float PunctuationPause = 0.4f;
    public float CharacterTimer = 0f;
    public int RemainingCharacterCount;
    private bool IsAnimating => RemainingCharacterCount > 0;

    public float PromptCountdownTimer = 0.6f;

    private Action DismissAction;

    public void Show(Action dismissAction)
    {
        DismissAction = dismissAction;
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        UiController.StartUiInteraction();
        DialogueText.ForceMeshUpdate(ignoreActiveState: true);
        DialogueText.maxVisibleCharacters = 0;
        CharacterTimer = SecondsPerCharacter;
        RemainingCharacterCount = DialogueText.textInfo.characterCount;

        ContinuePrompt.SetActive(false);
    }

    private void Update()
    {
        if (IsAnimating)
        {
            CharacterTimer -= Time.deltaTime;
            if (CharacterTimer < 0f)
            {
                bool isPunctuation = DialogueText.textInfo.characterInfo[DialogueText.maxVisibleCharacters].character is '.' or '!' or '?';
                CharacterTimer = isPunctuation ? PunctuationPause : SecondsPerCharacter;

                RemainingCharacterCount--;
                DialogueText.maxVisibleCharacters++;

                if (!isPunctuation)
                    SoundEffectsController.Instance.PlayChirp(ChirpRandomness, ChirpFont);
            }

            // Allow skipping the animation
            if (UiController.CheckGlobalDismiss())
            {
                RemainingCharacterCount = 0;
                DialogueText.maxVisibleCharacters = int.MaxValue;
                PromptCountdownTimer = 0f;
            }
        }
        else if (PromptCountdownTimer >= 0f)
        {
            PromptCountdownTimer -= Time.deltaTime;
            if (PromptCountdownTimer < 0f)
                ContinuePrompt.SetActive(true);
        }
        else if (UiController.CheckGlobalDismiss())
        {
            gameObject.SetActive(false);
            DismissAction?.Invoke();
            UiController.EndUiInteraction();
        }
    }
}
