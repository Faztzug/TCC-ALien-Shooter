using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteTrigger : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter(Collider other) 
    {
        if(!other.CompareTag("Player"))
        {
            var curTransform = other.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            while (healthObj == null && curTransform.parent != null)
            {
                Debug.Log("while pass");
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
            }
            healthObj?.UpdateHealth(damage);
            if(healthObj) Debug.Log("BITE SUCESS: " + other.name);
            else Debug.Log("BITE FAILLLL: " + other.name);
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
