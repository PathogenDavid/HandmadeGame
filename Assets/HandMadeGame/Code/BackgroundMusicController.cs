using UnityEngine;
using UnityEngine.Audio;

public sealed class BackgroundMusicController : MonoBehaviour
{
    private static BackgroundMusicController _Instance;
    public static BackgroundMusicController Instance
    {
        get
        {
            if (_Instance is not null)
                return _Instance;

            return _Instance = FindObjectOfType<BackgroundMusicController>();
        }
    }

    public bool EnableDebugger;

    public AudioMixerSnapshot MainMenuSnapshot;
    public AudioMixerSnapshot ExplorationSnapshot;
    public AudioMixerSnapshot CharacterSnapshot;
    public AudioMixerSnapshot DecoratingSnapshot;

    public AudioSource MainMenuLeadIn;
    public AudioSource MainMenuLoop;

    public AudioSource[] CharacterSources;

    public float TransitionTime = 1f;

    private void Awake()
    {
        Application.runInBackground = true;

        // This doesn't seem quite perfect but it's good enough for a game jam
        // Without first-class support for having the audio thread actually do this I don't think it'll ever be perfect
        double startTime = AudioSettings.dspTime + 0.3;
        MainMenuLeadIn.PlayScheduled(startTime);
        double leadInDuration = (double)(MainMenuLeadIn.clip.samples + 1) / (double)MainMenuLeadIn.clip.frequency;
        MainMenuLoop.PlayScheduled(startTime + leadInDuration);
    }

    public void TransitionToMainMenuMusic()
        => MainMenuSnapshot.TransitionTo(TransitionTime);

    public void TransitionToExplorationMusic()
        => ExplorationSnapshot.TransitionTo(TransitionTime);

    public void TransitionToCharacterMusic()
        => CharacterSnapshot.TransitionTo(TransitionTime);

    public void TransitionToCharacterMusic(AudioClip characterClip)
    {
        bool found = false;
        foreach (AudioSource characterSource in CharacterSources)
        {
            if (characterSource.clip == characterClip)
            {
                found = true;
                characterSource.mute = false;
            }
            else
            {
                characterSource.mute = true;
            }
        }

        if (!found)
            Debug.LogError($"Tried to transition to unregistered character music '{characterClip.name}'");

        TransitionToCharacterMusic();
    }

    public void TransitionToDecoratingMusic()
        => DecoratingSnapshot.TransitionTo(TransitionTime);

    private void OnGUI()
    {
        if (!EnableDebugger)
            return;

        if (GUILayout.Button("MainMenu"))
            TransitionToMainMenuMusic();
        if (GUILayout.Button("Exploration"))
            TransitionToExplorationMusic();
        if (GUILayout.Button("Character"))
            TransitionToCharacterMusic();
        if (GUILayout.Button("Decorating"))
            TransitionToDecoratingMusic();

        foreach (AudioSource source in CharacterSources)
        {
            if (GUILayout.Button(source.clip.name))
                TransitionToCharacterMusic(source.clip);
        }
    }
}
