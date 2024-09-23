using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenu : MonoBehaviour
{
    public Button StartButton;

    public BirdController BirdController;
    public Rigidbody BirdRigidBody;
    public Animator BirdAnimator;
    public FollowPlayer CameraController;
    public Prologue Prologue;
    public Transform BirdRevealDestination;
    public InventoryHotBarController InventoryHotBar;
    public BirdFlyInAnimation BirdFlyInAnimation;

    public AudioClip ButtonClickSound;
    public AudioClip TreeRustleSound;
    public float TreeRustleSoundTriggerPoint = 0.9f;

    public CanvasGroup CanvasGroup;
    public float FadeOutTime = 3f;
    public float BirdRevealDelay = 0.5f;
    public float BirdRevealTime = 0.25f;
    public float BirdRevealOvershoot = 0.005f;
    public float PrologueDelay = 0.5f;

    private void Awake()
    {
        BirdController.enabled = false;
        BirdRigidBody.isKinematic = true;
        // Interpolation causes jitter on WebGL builds
        RigidbodyInterpolation oldBirdRigidBodyInterpolation = BirdRigidBody.interpolation;
        BirdRigidBody.interpolation = RigidbodyInterpolation.None;
        BirdAnimator.enabled = false;
        CameraController.enabled = false;
        InventoryHotBar.Hide();

        StartButton.onClick.AddListener(() =>
        {
            SoundEffectsController.Instance.PlayUiSound(ButtonClickSound);

            CanvasGroup.interactable = false;
            StartCoroutine(StartGameCoroutine());

            IEnumerator StartGameCoroutine()
            {
                // Fade out the main menu
                float timer = FadeOutTime;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    CanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, timer / FadeOutTime);
                    yield return null;
                }

                // Pause for a bit
                timer = BirdRevealDelay;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                // Bring out the birb
                Vector3 startPosition = BirdController.transform.position;
                Vector3 destinationPosition = BirdRevealDestination.position;
                timer = BirdRevealTime;

                bool didRustle = false;

                void BirdAnimate()
                {
                    float t = 1f - timer / BirdRevealTime;
                    t *= t;
                    t = Mathf.Clamp(t, 0f, 1f + BirdRevealOvershoot);

                    if (!didRustle && t > TreeRustleSoundTriggerPoint)
                    {
                        didRustle = true;
                        SoundEffectsController.Instance.PlayUiSound(TreeRustleSound); //TODO: Don't use UI sounds for this
                    }

                    BirdController.transform.position = Vector3.LerpUnclamped(startPosition, destinationPosition, t);
                }

                while (timer > -BirdRevealOvershoot)
                {
                    timer -= Time.deltaTime;
                    BirdAnimate();
                    yield return null;
                }

                while (timer < 0f)
                {
                    timer += Time.deltaTime;
                    BirdAnimate();
                    yield return null;
                }

                BirdController.transform.position = destinationPosition;

                // Pause for a bit
                timer = PrologueDelay;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                // Show the prologue
                Prologue.Show(() =>
                {
                    BirdAnimator.enabled = true;
                    BackgroundMusicController.Instance.TransitionToExplorationMusic();
                    BirdFlyInAnimation.StartAnimation(() =>
                    {
                        BirdController.enabled = true;
                        BirdRigidBody.interpolation = oldBirdRigidBodyInterpolation;
                        BirdRigidBody.isKinematic = false;
                        CameraController.enabled = true;
                        InventoryHotBar.Show();
                    });
                });

                UiController.EndUiInteraction();
                gameObject.SetActive(false);
            }
        });
    }

    private void Start()
        => UiController.StartUiInteraction();
}
