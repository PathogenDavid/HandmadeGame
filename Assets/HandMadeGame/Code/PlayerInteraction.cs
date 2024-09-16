using System;
using UnityEngine;

public sealed class PlayerInteraction : MonoBehaviour
{
    public ControllerPrompt ControllerPrompt;

    public Vector3 InteractionCenter;
    public float InteractionRadius;
    private Vector3 InteractionCenterGlobal => transform.position + InteractionCenter;

    public LayerMask InteractionMask = 1 << 3;

    private Collider CurrentFocus;

    private bool Suppressed;

    private void OnDrawGizmosSelected()
        => Gizmos.DrawWireSphere(InteractionCenterGlobal, InteractionRadius);

    private void Awake()
    {
        UiController.UiInteractionStart += () =>
        {
            Suppressed = true;
            CurrentFocus = null;
            ControllerPrompt.Hide();
        };

        UiController.UiInteractionEnd += () => Suppressed = false;
    }

    private Collider[] _Colliders = new Collider[8];
    private void Update()
    {
        if (Suppressed)
            return;

        // Update current focus by finding the nearest collider
        int count = Physics.OverlapSphereNonAlloc(InteractionCenterGlobal, InteractionRadius, _Colliders, InteractionMask);
        Collider closestCollider = null;
        float closestSquaredDistance = float.PositiveInfinity;
        foreach (Collider collider in new Span<Collider>(_Colliders, 0, count))
        {
            float squaredDistance = (collider.transform.position - InteractionCenterGlobal).sqrMagnitude;
            if (squaredDistance < closestSquaredDistance)
                closestCollider = collider;
        }

        // Record the new focus if it changed and hide the prompt if we have no focus
        bool isNewFocus = closestCollider != CurrentFocus;
        if (isNewFocus)
        {
            CurrentFocus = closestCollider;

            if (CurrentFocus == null)
                ControllerPrompt.Hide();
        }

        // Won't show input prompt or handle interaction when there's no focus
        if (CurrentFocus == null)
            return;

        // If it's not a new focus and the player isn't trying to interact, we have nothing to do
        bool interactionTriggered = Input.GetKeyDown(UiController.InteractionKey);
        if (!isNewFocus && !interactionTriggered)
            return;

        // Either update the prompt as appropriate or handle the interaction
        if (CurrentFocus.TryGetComponent(out Quest quest))
        {
            ControllerPrompt.Show(UiController.InteractionKey, "Talk");
            if (interactionTriggered)
                GameFlow.Instance.HandleInteraction(quest);
        }
        else if (CurrentFocus.TryGetComponent(out NestItem item))
        {
            ControllerPrompt.Show(UiController.InteractionKey, $"Pick up {item.name}");
            if (interactionTriggered)
                GameFlow.Instance.HandleInteraction(item);
        }
#if DEBUG
        else
        {
            Debug.Log($"Object '{CurrentFocus.name}' is marked as interactable but has no known interactable components!");
        }
#endif
    }
}
