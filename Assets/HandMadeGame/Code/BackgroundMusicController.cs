using UnityEngine;
using UnityEngine.Audio;

public sealed class BackgroundMusicController : MonoBehaviour
{
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

    private void OnGUI()
    {
        if (!EnableDebugger)
            return;

        if (GUILayout.Button("MainMenu"))
            MainMenuSnapshot.TransitionTo(TransitionTime);
        if (GUILayout.Button("Exploration"))
            ExplorationSnapshot.TransitionTo(TransitionTime);
        if (GUILayout.Button("Character"))
            CharacterSnapshot.TransitionTo(TransitionTime);
        if (GUILayout.Button("DecoratingSnapshot"))
            DecoratingSnapshot.TransitionTo(TransitionTime);

        foreach (AudioSource source in CharacterSources)
        {
            if (GUILayout.Button(source.clip.name))
            {
                foreach (AudioSource characterSource in CharacterSources)
                    characterSource.mute = true;

                source.mute = false;
                CharacterSnapshot.TransitionTo(TransitionTime);
            }
        }
    }
}
