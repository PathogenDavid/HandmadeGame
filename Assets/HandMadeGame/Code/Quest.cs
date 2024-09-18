using UnityEngine;

public sealed class Quest : MonoBehaviour
{
    public Sprite CharacterPortrait;
    public int RequiredReputation;
  public AudioClip CharacterMusic;
  public AudioClip[] CharacterTalkSounds;

    [Header("Quest Logistics")]
    [Multiline] public string StartDialogue = "HELLO YES I WOULD LIKE SOME DECORATION";
    [Multiline] public string QuestAcceptedDialogue = "THANK YOU, PLEASE FIND [number] ITEMS";
    [Multiline] public string QuestDescription = "FIND [number] ITEMS";
    [Multiline] public string QuestRejectedDialogue = "WOW ARE YOU A BIRDECORATOR OR NOT?";

    [Header("Quest Unavailable")]
    public string ReputationRequirementNotMetDialogue = "Oh, I don’t think you’re ready to decorate my nest…";
    public string QuestAlreadyCompletedDialogue = "Thanks for all your hard work! I love how you decorated my nest!";

    [Header("Ready to Begin?")]
    [Multiline] public string ReadyToBeginDialogue = "ARE YOU READY TO BEGIN?";
    [Multiline] public string NotReadyToBeginDialogue = "OH, WELL PLEASE HURRY THIS IS AN INTERIOR DESIGN EMERGENCY! DO YOU NEED ME TO REPEAT MYSELF?";
    [Multiline] public string NoNeedToRepeatDialogue = "OK JUST LET ME KNOW!";

    [Header("Puzzle Outcomes")]
    [Multiline] public string QuestCompleteDialogue = "IT'S PERFECT THANK YOU";
    [Multiline] public string WrongItemsDialogue = "SOME OF THESE ITEMS DON'T SEEM QUITE RIGHT";
    [Multiline] public string NotEnoughItemsDialogue = "NOT ENOUGH ITEMS";
    [Multiline] public string WrongItemArrangementDialogue = "THE FENG SHUI OF THIS NEST IS ALL WRONG";

    [Header("Quest State")]
    public QuestState CurrentState = QuestState.NotStarted;
    public NestItem[,] Board;
    public readonly int BoardWidth = 3;
    public readonly int BoardHeight = 3;

    [Header("Items to spawn")]
    public GameObject[] SpawnOnAccept;

    [HideInInspector]
    public QuestValidatorBase Validator;

    private void Awake()
    {
        CurrentState = QuestState.NotStarted;
        Board = new NestItem[BoardWidth, BoardHeight];

        Validator = GetComponent<QuestValidatorBase>();
        if (Validator == null)
        {
            Debug.LogError($"Quest '{name}' does not have a validator!");
            Validator = gameObject.AddComponent<NullQuestValidator>();
        }

        // Ensure spawn items are hidden at start
        foreach (GameObject gameObject in SpawnOnAccept)
            gameObject.SetActive(false);
    }

  public AudioClip GetRandomTalkSound()
  {
    return CharacterTalkSounds[UnityEngine.Random.Range(0, CharacterTalkSounds.Length)];
  }
}
