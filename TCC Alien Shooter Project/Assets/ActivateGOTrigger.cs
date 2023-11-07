using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGOTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> objets = new List<GameObject>();
    [SerializeField] private Sound musicToChange;

    private void Awake()
    {
        foreach (var go in objets) go.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var go in objets) go.SetActive(true);
            if(musicToChange.clip != null)
            {
                FindAnyObjectByType<MusicPlayer>().ChangeMusic(musicToChange);
            }
        }
    }
}
