using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip gunFire;
    public AudioClip upgradedGunFire;
    public AudioClip hurt;
    public AudioClip alienDeath;
    public AudioClip marineDeath;
    public AudioClip victory;
    public AudioClip elevatorArrived;
    public AudioClip powerUpPickup;
    public AudioClip powerUpAppear;
    public static SoundManager Instance = null; //Creates a static instance
    private AudioSource soundEffectAudio;      //Audio source added to the sound manager used to play sound effects
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)           //This if and else if statement is used to create a simpleton and
        {                               //makes sure only one static instance ever exists
            Instance = this; 
        }
        else if (Instance != this)     
        {
            Destroy(gameObject);
        }

        AudioSource[] sources = GetComponents<AudioSource>(); //Gets the number of audio sources
        foreach(AudioSource source in sources)
        {
            if (source.clip == null)       //Selects the audio source with no sound clip stored in it  to it
            {
                soundEffectAudio = source;  //Adds this sound clip to this specific Audio Source
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
