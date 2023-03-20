using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0, 1)]
    public float volume = 1f;
    [Range(-3, 3)]
    public float pitch = 1f;
    [Range(-3, 3)]
    public float[] pitchRNG = new float[2]{0,0};
    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource audioSource;
    
    public void Setup(AudioSource audioSource)
    {
        audioSource.clip = clip;
        if(GameState.GameStateInstance != null)
        {
            audioSource.volume = GameState.SaveData.mute ? 0f : volume * GameState.SaveData.sfxVolume;
        }
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.playOnAwake = playOnAwake;
    }

    public void PlayOn(AudioSource audioSource)
    {
        if(GameState.GameStateInstance != null && GameState.SaveData.mute) return;
        
        Setup(audioSource);
        if(pitchRNG.Length >= 2 && pitchRNG[0] != pitchRNG[1])
        {
            audioSource.pitch = pitch + Random.Range(pitchRNG[0], pitchRNG[1]);
        }
        audioSource.PlayOneShot(clip);
    }

}
