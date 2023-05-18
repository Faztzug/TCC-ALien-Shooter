using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterBite : DamageHealthCollider
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<BiteTrigger>(out BiteTrigger bite))
        {
            var health = (ShieldHealth)GetHealth(bite.gameObject);
            health?.PierciShieldDamage(damage);
        }
    }
}
