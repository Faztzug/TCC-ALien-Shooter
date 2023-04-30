using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDamage : DamageHealthCollider
{
    private void OnTriggerStay(Collider other) 
    {
        GetHealth(other)?.UpdateHealth(damage * Time.deltaTime);
    }
}
