using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IddleSound : MonoBehaviour
{
    [SerializeField] private Sound sound;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        Play();
    }

    public void Play()
    {
        sound.PlayOn(audioSource);
    }
}
