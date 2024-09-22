using UnityEngine;
using Random = System.Random;

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
    public AudioSource BirdChirpAudioSource;
    public float ChirpPitchVariance = 0.2f;

    public void PlayUiSound(AudioClip uiSound)
        => UiAudioSource.PlayOneShot(uiSound);

    public void PlayChirp(Random random, AudioClip[] chirpFont)
    {
        // Skip chirps when the font is empty
        if (chirpFont == null || chirpFont.Length == 0)
            return;

        // Select random chirp / pitch
        // (This is intentionally before the isPlaying check so that it affects the random state even if the chirp is skipped.)
        AudioClip chirp = chirpFont[random.Next(0, chirpFont.Length)];
        float pitch = 1f + (float)(random.NextDouble() * 2.0 - 1.0) * ChirpPitchVariance;

        // Skip chirps when one is currently playing
        if (BirdChirpAudioSource.isPlaying)
            return;

        // Play a random chirp at a random pitch
        BirdChirpAudioSource.clip = chirp;
        BirdChirpAudioSource.pitch = pitch;
        BirdChirpAudioSource.Play();
    }
}
