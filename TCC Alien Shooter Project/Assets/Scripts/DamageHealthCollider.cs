using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHealthCollider : MonoBehaviour
{
    [SerializeField] protected float damage;
    private List<Health> lastDamages = new List<Health>();

    protected Health GetHealth(Collider other) => GetHealth(other.gameObject);
    protected Health GetHealth(GameObject gameObject)
    {
        var curTransform = gameObject.transform;
        var healthObj = curTransform.GetComponentInChildren<Health>();
        while (healthObj == null && curTransform.parent != null)
        {
            curTransform = curTransform.parent;
            healthObj = curTransform.GetComponent<Health>();
        }
        if(healthObj != null && !lastDamages.Contains(healthObj))
        {
            lastDamages.Add(healthObj);
            StartCoroutine(EndEnemyInvicibility(healthObj));
        }
        return healthObj;
    }

    IEnumerator EndEnemyInvicibility(Health healthObj)
    {
        yield return new WaitForEndOfFrame();
        lastDamages.Remove(healthObj);
    }
    
    private void OnValidate() 
    {
        if(damage > 0) damage = -damage;
    }
}
