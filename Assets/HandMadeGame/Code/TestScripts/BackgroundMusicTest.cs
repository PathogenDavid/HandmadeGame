using UnityEngine;

public class BackgroundMusicTest : MonoBehaviour
{
    public AudioClip Music;

    void Start()
    {
        Audio.SetMusicVolume(7);
        Audio.PlayMusic(Music, 0f);
    }
}
