using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteTrigger : MonoBehaviour
{
    [SerializeField] float damage;
    private List<Health> lastDamages = new List<Health>();

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
            if(healthObj != null && !lastDamages.Contains(healthObj))
            {
                healthObj?.UpdateHealth(damage);
                lastDamages.Add(healthObj);
                StartCoroutine(EndEnemyInvicibility(healthObj));
                Debug.Log("BITE SUCESS: " + other.name);
            }
            else Debug.Log("BITE FAILLLL: " + other.name);
            
        } 
    }

    IEnumerator EndEnemyInvicibility(Health healthObj)
    {
        yield return new WaitForEndOfFrame();
        lastDamages.Remove(healthObj);
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
