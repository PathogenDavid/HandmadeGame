using UnityEngine;

public sealed class DummyArrangementController : ArrangementModeControllerBase
{
    private Quest Target;
    private NestItem CurrentNestItem = null;
    private NestItem[] NestItems;

    private void Awake()
        => NestItems = GetComponentsInChildren<NestItem>();

    public override void StartArrangementMode(Quest quest)
    {
        Debug.Assert(Target == null);
        UiController.StartUiInteraction();
        Target = quest;
    }

    private void OnGUI()
    {
        if (Target == null)
            return;

        GUILayout.BeginArea(new Rect(280f, 0f, 120f, 1000f));

        GUILayout.Label($"Dummy arrangement UI");

        GUILayout.Label($"Quest: {Target}");
        GUILayout.Label($"Verdict: {Target.Validator.CheckPuzzle(Target)}");

        GUILayout.Label("Items");
        if (GUILayout.Button($"Nothing{(CurrentNestItem == null ? "*" : "")}"))
            CurrentNestItem = null;

        foreach (NestItem item in NestItems)
        {
            if (GUILayout.Button($"{item.name}{(CurrentNestItem == item ? "*" : "")}"))
                CurrentNestItem = item;
        }

        for (int y = 0; y < Target.BoardHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < Target.BoardWidth; x++)
            {
                NestItem currentItem = Target.Board[x, y];
                if (GUILayout.Button(currentItem == null ? " " : currentItem.name[0].ToString()))
                    Target.Board[x, y] = CurrentNestItem;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Exit"))
        {
            GameFlow.Instance.EndArrangementMode(Target);
            Target = null;
            UiController.EndUiInteraction();
        }

        GUILayout.EndArea();
    }
}
