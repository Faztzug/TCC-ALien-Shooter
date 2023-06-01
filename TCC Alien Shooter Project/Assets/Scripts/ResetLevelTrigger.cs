using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            //SceneManager.LoadScene(gameObject.scene.name);
            other.GetComponent<PlayerShieldHealth>().PierciShieldDamage(-1000);
        }
    }
}
