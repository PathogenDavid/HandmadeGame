using UnityEngine;

public class BackgroundMusicTest : MonoBehaviour
{
    public AudioClip Music;

    void Start()
    {
        Audio.SetAudioVolume(7);
        Audio.PlayMusic(Music, 0f);
    }
}
