using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FlapScript : MonoBehaviour
{
    public AudioClip[] flapSounds;

    private Random random;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallFlap() {
        random = new Random();
        FindObjectOfType<SoundEffectsController>().PlayFlap(random, flapSounds);
    }
}
