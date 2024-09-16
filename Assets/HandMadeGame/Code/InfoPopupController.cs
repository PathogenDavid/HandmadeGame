using System;
using TMPro;
using UnityEngine;

public sealed class InfoPopupController : MonoBehaviour
{
    public TMP_Text Text;
    public ControllerPrompt ControllerPrompt;
    private Action DismissAction;

    private static Action NoOp = () => { };

    private void Awake()
        => gameObject.SetActive(false);

    public void ShowPopup(string text, Action dismissAction)
    {
        Text.text = UiController.ProcessDisplayString(text);
        gameObject.SetActive(true);
        DismissAction = dismissAction;
        UiController.StartUiInteraction();
        ControllerPrompt.Show(UiController.InteractionKey, "Dismiss");
    }

    public void ShowPopup(string text)
        => ShowPopup(text, NoOp);

    private void Update()
    {
        if (gameObject.activeInHierarchy && DismissAction != null)
        {
            if (UiController.CheckGlobalDismiss())
            {
                ControllerPrompt.Hide();
                gameObject.SetActive(false);
                UiController.EndUiInteraction();
                DismissAction();
            }
        }
    }
}
