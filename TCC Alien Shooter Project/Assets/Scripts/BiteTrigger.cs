using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteTrigger : DamageHealthCollider
{
    private void OnTriggerEnter(Collider other) 
    {
        if(!other.CompareTag("Player"))
        {
            GetHealth(other.gameObject)?.UpdateHealth(damage);
        } 
    }
}
