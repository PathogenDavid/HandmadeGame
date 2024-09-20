using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    public AudioClip MusicIN;
    public AudioClip MusicLOOP;
    public AudioClip MusicEXPLORE;

    public void Start()
    {
        Audio.SetMusicVolume(4);
        Audio.PlayMusic(MusicIN);
        Audio.QueueNextSong(MusicLOOP);
    }

    public void Update()
    {
        if (Input.anyKey)
        {
            Debug.Log("NEW SONG");
            Audio.PlayMusic(MusicEXPLORE);
        }
    }
}
