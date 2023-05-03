using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : DamageHealthCollider
{
    private void OnTriggerEnter(Collider other) 
    {
        GetHealth(other.gameObject)?.UpdateHealth(damage, DamageType.NULL);
    }

    private void OnCollisionEnter(Collision other) 
    {
        GetHealth(other.gameObject)?.UpdateHealth(damage, DamageType.NULL);
    }
}
