using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterBite : DamageHealthCollider
{
    [SerializeField] protected DamageType damageType = DamageType.eletricDamage;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<BiteTrigger>(out BiteTrigger bite))
        {
            var health = (ShieldHealth)GetHealth(bite.gameObject);
            health?.PierciShieldDamage(damage, damageType);
        }
    }
}
