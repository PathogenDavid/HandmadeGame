using System;
using UnityEngine;

public sealed class BirdFlyInAnimation : MonoBehaviour
{
    public Transform Visuals;
    public Transform Destination;

    public FollowPlayer CameraController;

    private Vector3 Velocity = Vector3.zero;
    public float PositionSmoothing = 0.15f;
    public float OrientationSmoothing = 4f;

    private float TotalDistance;
    private Action FinishAction;

    public float CameraEnableDistancePercent = 0.9f;

    public AudioClip TreeRustleSound;
    public float TreeRustleTimer = 0.1f;

    public void StartAnimation(Action finishAction)
    {
        enabled = true;
        TotalDistance = (Destination.position - transform.position).magnitude;
        FinishAction = finishAction;
    }

    private void Update()
    {
        if (TreeRustleTimer > 0f)
        {
            TreeRustleTimer -= Time.deltaTime;
            if (TreeRustleTimer <= 0f)
                SoundEffectsController.Instance.PlayUiSound(TreeRustleSound); //TODO: Don't use UI sounds for this
        }

        transform.position = Vector3.SmoothDamp(transform.position, Destination.position, ref Velocity, PositionSmoothing);
        Visuals.rotation = Quaternion.Slerp(Visuals.rotation, Destination.rotation, OrientationSmoothing * Time.deltaTime);

        float distanceRemaining = (Destination.position - transform.position).magnitude;
        if (distanceRemaining < TotalDistance * (1f - CameraEnableDistancePercent) && !CameraController.enabled)
        {
            CameraController.enabled = true;
        }
        else if (distanceRemaining < 0.01f)
        {
            CameraController.enabled = true;
            FinishAction?.Invoke();
            enabled = false;
        }
    }
}
