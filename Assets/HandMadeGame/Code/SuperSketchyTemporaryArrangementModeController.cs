using UnityEngine;

public sealed class SuperSketchyTemporaryArrangementModeController : ArrangementModeControllerBase
{
    private Quest Target;
    private NestItem CurrentNestItem = null;

    public override void StartArrangementMode(Quest quest)
    {
        Debug.Assert(Target == null);
        Debug.Assert(CurrentNestItem == null);
        UiController.StartUiInteraction();
        Target = quest;
    }

    private void OnGUI()
    {
        if (Target == null)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 200f, 0f, 200f, Screen.height));

        GUILayout.Label($"Last-second prototype puzzle solving UI because the real one wasn't quite finished sorry <3");
        
        GUILayout.Space(10f);
        GUILayout.Label("== Inventory ==");

        NestItem putBack = null;
        NestItem removeItem = null;
        foreach (NestItem item in GameFlow.Instance.Inventory)
        {
            if (GUILayout.Button(item.name))
            {
                if (CurrentNestItem != null)
                {
                    Debug.Assert(putBack == null);
                    Debug.Assert(removeItem == null);
                    putBack = CurrentNestItem;
                }

                CurrentNestItem = removeItem = item;
            }
        }

        if (putBack != null)
            GameFlow.Instance.Inventory.Add(putBack);

        if (removeItem != null)
            GameFlow.Instance.Inventory.Remove(removeItem);

        if (CurrentNestItem != null)
        {
            GUILayout.Space(10f);
            if (GUILayout.Button($"Put {CurrentNestItem.name} back"))
            {
                GameFlow.Instance.Inventory.Add(CurrentNestItem);
                CurrentNestItem = null;
            }
        }

        GUILayout.EndArea();

        const float w = 200f;
        const float h = 200f;
        float left = Screen.width * 0.5f - (w * Target.BoardWidth * 0.5f);
        float top = Screen.height * 0.5f - (h * Target.BoardHeight * 0.5f);
        for (int y = 0; y < Target.BoardHeight; y++)
        {
            for (int x = 0; x < Target.BoardWidth; x++)
            {
                Rect rect = new(left + x * w, top + y * h, w, h);
                NestItem currentItem = Target.Board[x, y];
                if (GUI.Button(rect, currentItem == null ? "" : currentItem.name))
                {
                    Target.Board[x, y] = CurrentNestItem;
                    CurrentNestItem = currentItem;
                }
            }
        }

        if (GUI.Button(new Rect(left, top + 3 * h, w * 3, 30f), "Exit"))
        {
            if (CurrentNestItem != null)
            {
                GameFlow.Instance.Inventory.Add(CurrentNestItem);
                CurrentNestItem = null;
            }

            GameFlow.Instance.EndArrangementMode(Target);
            Target = null;
            UiController.EndUiInteraction();
        }
    }
}
