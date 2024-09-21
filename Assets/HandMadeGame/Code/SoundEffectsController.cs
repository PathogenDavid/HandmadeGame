using UnityEngine;

public sealed class SoundEffectsController : MonoBehaviour
{
    private static SoundEffectsController _Instance;
    public static SoundEffectsController Instance
    {
        get
        {
            if (_Instance is not null)
                return _Instance;

            return _Instance = FindObjectOfType<SoundEffectsController>();
        }
    }

    public AudioSource UiAudioSource;

    public void PlayUiSound(AudioClip uiSound)
        => UiAudioSource.PlayOneShot(uiSound);
}
