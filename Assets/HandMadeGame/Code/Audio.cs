using System.Collections;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private static Audio _Singleton;
    public static Audio Singleton
    {
        get
        {
            if (_Singleton is not null)
                return _Singleton;

            return _Singleton = FindObjectOfType<Audio>();
        }
    }


    private float MusicVolume = 0;
    private float SFXVolume = 0;

    private AudioSource CurrentMusicSource;
    private AudioSource PreviousMusicSource;

    private AudioSource SfxSource;
    private AudioSource SfxSourcePitched;
    private bool MusicCrossfadeActive;
    private bool QueueVolumeLock = false;
    private float QueueCrossoverTime;

    private const float DefaultCrossFadeTime = 10f;

    private void Awake()
    {
        if (Singleton != this)
        {
            Debug.LogError($"Multiple {nameof(Audio)} instances exist in the scene!");
            Destroy(this);
            return;
        }

        // Create two music audio sources, this will allow the script to cross-fade between two songs
        int musicSourceIndex = 0;
        AudioSource CreateMusicSource()
        {
            GameObject NewMusicSource = new GameObject($"MusicSource{++musicSourceIndex}");
            AudioSource source = NewMusicSource.AddComponent<AudioSource>();
            source.loop = true;
            NewMusicSource.transform.parent = transform;
            return source;
        }

        CurrentMusicSource = CreateMusicSource();
        PreviousMusicSource = CreateMusicSource();

        // Create a new gameobject with AudioSource to handle sound effects
        GameObject NewSfxSource = new GameObject("SFXSource");
        SfxSource = NewSfxSource.AddComponent<AudioSource>();
        NewSfxSource.transform.parent = transform;

        // Create a new gameobject with AudioSource to handle sound effects
        GameObject NewSfxSourcePitched = new GameObject("SFXSourcePitched");
        SfxSourcePitched = NewSfxSourcePitched.AddComponent<AudioSource>();
        NewSfxSourcePitched.transform.parent = transform;
    }

    private void SwapMusicSources()
        => (CurrentMusicSource, PreviousMusicSource) = (PreviousMusicSource, CurrentMusicSource);

    // This will force stop all sound effects
    public void StopAllSFX() => SfxSource.Stop();

    public void StopMusic(float _fade = 0)
    {
        SwapMusicSources();
        CurrentMusicSource.clip = null;
        DoCrossfade(_fade);
    }

    public static void SetMusicVolume(int Vol) => Audio.Singleton.SetMusicVolumeSingle(Vol);
    public void SetMusicVolumeSingle(int Vol)
    {
        Vol = Mathf.Clamp(Vol, 0, 10);
        MusicVolume = (float)Vol / 10;
        if (!MusicCrossfadeActive)
        {
            CurrentMusicSource.volume = MusicVolume;
            PreviousMusicSource.volume = 0;
        }
    }

    public static void SetSFXVolume(int Vol) => Audio.Singleton.SetSFXVolumeSingle(Vol);
    public void SetSFXVolumeSingle(int Vol)
    {
        Vol = Mathf.Clamp(Vol, 0, 10);
        SFXVolume = (float)Vol / 10;
        SfxSource.volume = SFXVolume;
        SfxSourcePitched.volume = SFXVolume;
    }

    // Returns the currently playing song
    public static AudioClip CurrentSong() { return Audio.Singleton.CurrentSongSingle(); }
    public AudioClip CurrentSongSingle() { return CurrentMusicSource.clip; }
    public float CurrentSongPosition() { return CurrentMusicSource.time; }
    public void PauseCurrentSong() { CurrentMusicSource.Pause(); }
    public void UnpauseCurrentSong() { CurrentMusicSource.UnPause(); }

    public static void QueueNextSong(AudioClip _clip) => Audio.Singleton.QueueNextSongSingle(_clip);
    public void QueueNextSongSingle(AudioClip _clip)
    {
        QueueVolumeLock = true;
        CurrentMusicSource.loop = false;
        QueueCrossoverTime = Time.time + CurrentMusicSource.clip.length;
        PreviousMusicSource.clip = _clip;
        PreviousMusicSource.PlayDelayed(CurrentMusicSource.clip.length - 0.06f); // Hard coded time to take into account song loading? It works OK enough
        PreviousMusicSource.volume = MusicVolume;
    }

    // This will load the referenced AudioClip and crossfade between them.
    // This also starts the music at the same timestamp as the currently playing song.
    // Great for music that has different layers!
    public static void PlayMusic(AudioClip _clip, float _fade = DefaultCrossFadeTime) => Audio.Singleton.PlayMusicSingle(_clip, _fade);
    public void PlayMusicSingle(AudioClip _clip, float _fade = DefaultCrossFadeTime)
    {
        CurrentMusicSource.loop = true;
        PreviousMusicSource.loop = true;

        if (QueueVolumeLock)
        {
            QueueVolumeLock = false;
            if (QueueCrossoverTime < Time.time)
            {
                SwapMusicSources();
                CurrentMusicSource.volume = 0;
            }
            else
            {
                PreviousMusicSource.volume = 0;
            }
        }

        // If the requested audio is different from the currently playing audio, start fade
        if (CurrentMusicSource.clip != _clip)
        {
            SwapMusicSources();
            CurrentMusicSource.clip = _clip;
            CurrentMusicSource.timeSamples = Mathf.Clamp(PreviousMusicSource.timeSamples, 0, CurrentMusicSource.clip.samples);
            CurrentMusicSource.Play();
            DoCrossfade(_fade);
        }
    }

    // Plays a one-shot sound
    public static void PlaySound(AudioClip _clip) { Audio.Singleton.PlaySoundSingle(_clip); }
    public void PlaySoundSingle(AudioClip _clip) { SfxSource.PlayOneShot(_clip, SFXVolume); }

    public static void PlaySoundPitched(AudioClip _clip, float PitchMod) { Audio.Singleton.PlaySoundPitchedSingle(_clip, PitchMod); }
    public void PlaySoundPitchedSingle(AudioClip _clip, float PitchMod)
    {
        SfxSourcePitched.pitch = PitchMod;
        SfxSourcePitched.PlayOneShot(_clip, SFXVolume);
    }

    public static void StopSFX() { Audio.Singleton.StopSFXSingle(); }
    public void StopSFXSingle() { SfxSource.Stop(); }

    private void DoCrossfade(float _duration)
    {
        // Don't crossfade if the previous track was null (IE: silence)
        if (PreviousMusicSource.clip == null)
            CurrentMusicSource.volume = MusicVolume;
        else
            StartCoroutine(AnimateMusicCrossfade(_duration));

        // Small coroutine that crossfades the currently playing song with the new one
        IEnumerator AnimateMusicCrossfade(float _duration)
        {
            MusicCrossfadeActive = true;
            float Percent = 0;
            while (Percent < 1)
            {
                Percent += Time.deltaTime / _duration;
                CurrentMusicSource.volume = Mathf.Lerp(0, MusicVolume, Percent);
                if (!QueueVolumeLock)
                {
                    PreviousMusicSource.volume = Mathf.Lerp(MusicVolume, 0, Percent);
                }
                else
                {
                    PreviousMusicSource.volume = MusicVolume;
                }
                yield return new WaitForFixedUpdate();
            }
            MusicCrossfadeActive = false;
            if (!QueueVolumeLock)
            {
                PreviousMusicSource.Stop();
            }
        }
    }
}