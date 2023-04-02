using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteTrigger : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("BITE: " + other.tag);
        if(!other.CompareTag("Player"))
        {
            var curTransform = other.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            while (healthObj != null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
            }
            healthObj?.UpdateHealth(damage);
        } 
    }

    // void OnTriggerStay(Collider other)
    // {
    //     if(!other.CompareTag("Player"))
    //     {
    //         Debug.Log("BITE: " + other.name);
    //         var curTransform = other.transform;
    //         var healthObj = curTransform.GetComponentInChildren<Health>();
    //         while (healthObj != null && curTransform.parent != null)
    //         {
    //             curTransform = curTransform.parent;
    //             healthObj = curTransform.GetComponent<Health>();
    //         }
    //         healthObj?.UpdateHealth(damage * Time.deltaTime);
    //     } 
    // }
}
