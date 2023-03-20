using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Sound soundOnCheckPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform respawnPositionTransform;
    private bool active;


    private void OnTriggerEnter(Collider other) 
    {
        if(active) return;
        if(other.CompareTag("Player"))
        {
            GameState.SetCheckPoint(respawnPositionTransform.position);
            animator.SetTrigger("Active");
            soundOnCheckPoint.PlayOn(audioSource);
            active = true;
        }
    }
}
