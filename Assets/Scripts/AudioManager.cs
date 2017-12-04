using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    public AudioClip[] SnowSounds;
    public AudioClip[] TreeThud;
    public AudioClip Heat;
    public AudioClip Bell;
    public AudioClip Pop;

    public AudioSource SnowSource;
    public AudioSource TreeThudSource;
    public AudioSource HeatSource;
    public AudioSource BellSource;
    public AudioSource PopSource;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySnowSound()
    {
        AudioSource audio = SnowSource;

        if(!audio.isPlaying)
        {
            audio.clip = SnowSounds[Random.Range(0, SnowSounds.Length)];
            audio.Play();
        }
    }

    public void PlayTreeThud()
    {
        AudioSource audio = TreeThudSource;

        if (!audio.isPlaying)
        {
            audio.clip = TreeThud[Random.Range(0, TreeThud.Length)];
            audio.Play();
        }
    }

    public void PlayHeat()
    {
        AudioSource audio = HeatSource;

        if (!audio.isPlaying)
        {
            audio.clip = Heat;
            audio.Play();
        }
    }

    public void PlayBell()
    {
        AudioSource audio = BellSource;

        if (!audio.isPlaying)
        {
            audio.clip = Bell;
            audio.Play();
        }
    }

    public void PlayPop()
    {
        AudioSource audio = PopSource;

        if (!audio.isPlaying)
        {
            audio.clip = Pop;
            audio.Play();
        }
    }
}
