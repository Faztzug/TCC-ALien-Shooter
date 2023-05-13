using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private Sound musicSound;
    private AudioClip CurrentPlaying;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musicSound.Setup(audioSource);

        UpdateVolume();

        audioSource.Play();
        CurrentPlaying = musicSound.clip;
        
        if(GameState.GameStateInstance != null) GameState.OnSettingsUpdated += UpdateVolume;
    }

    public void UpdateVolume()
    {
        if(this == null) return;
        if(GameState.GameStateInstance != null)
        {
            audioSource.volume = GameState.SettingsData.mute ? 0f : musicSound.volume * GameState.SettingsData.musicVolume;
        }
    }

    private void OnDestroy() 
    {
        GameState.OnSettingsUpdated -= UpdateVolume;
    }
}
