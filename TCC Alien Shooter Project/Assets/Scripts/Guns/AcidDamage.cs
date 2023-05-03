using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDamage : DamageHealthCollider
{
    private void OnTriggerStay(Collider other) 
    {
        var hp = GetHealth(other);
        hp?.UpdateHealth(damage * Time.deltaTime, DamageType.acidDamage);
    }
}
