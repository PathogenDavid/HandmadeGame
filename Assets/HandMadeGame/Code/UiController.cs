using System;
using UnityEngine;

public static class UiController
{
    public const KeyCode InteractionKey = KeyCode.E;

    /// <summary>Fires when a player inputs should be suppressed for UI interaction</summary>
    public static event Action UiInteractionStart;
    /// <summary>Fires when a player inputs can resume after UI interaction</summary>
    public static event Action UiInteractionEnd;

    public static int UiSessionDepth { get; private set; } = 0;

    public static void StartUiInteraction()
    {
        if (UiSessionDepth == 0)
            UiInteractionStart?.Invoke();

        UiSessionDepth++;
    }

    public static void EndUiInteraction()
    {
        Debug.Assert(UiSessionDepth > 0);
        UiSessionDepth--;

        if (UiSessionDepth == 0)
            UiInteractionEnd?.Invoke();
    }

    public static bool CheckGlobalDismiss()
        => Input.GetKeyDown(KeyCode.Space)
        || Input.GetKeyDown(KeyCode.Return)
        || Input.GetKeyDown(KeyCode.Escape)
        || Input.GetMouseButtonDown(0)
        || Input.GetKeyDown(InteractionKey);

    public static string ProcessDisplayString(string text)
        => text
        .Trim()
        .Replace("<em>", "<color=#fee761>")
        .Replace("</em>", "</color>")
        .Replace('’', '\'')
        .Replace("…", "...")
        ;
}
