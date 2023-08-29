using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Mathematics;

public class DamageHealthCollider : MonoBehaviour
{
    [SerializeField] protected float damage;
    public float Damage => damage;
    private List<Health> lastDamages = new List<Health>();
    protected virtual float InvicibilityTime => 0.01f;

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
        
        if(lastDamages.Contains(healthObj)) return null;
        if(healthObj != null && !lastDamages.Contains(healthObj))
        {
            Debug.Log(name + " doing damage on " + healthObj.gameObject.name);
            lastDamages.Add(healthObj);
            EndEnemyInvicibility(healthObj);
        }
        return healthObj;
    }

    protected async void EndEnemyInvicibility(Health healthObj)
    {
        await Task.Delay(Mathf.RoundToInt(InvicibilityTime * 1000));
        //Debug.Log("removing health of " + healthObj?.gameObject.ToString());
        if(this is null) return;
        lastDamages.Remove(healthObj);
    }
    
    private void OnValidate() 
    {
        if(damage > 0) damage = -damage;
    }
}
