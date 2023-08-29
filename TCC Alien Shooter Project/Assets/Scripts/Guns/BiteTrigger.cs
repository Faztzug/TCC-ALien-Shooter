using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BiteTrigger : DamageHealthCollider
{
    [HideInInspector] public PiranhaGun piranha;
    protected override float InvicibilityTime => 0.5f;
    private void OnTriggerEnter(Collider other) 
    {
        if(!other.CompareTag("Player"))
        {
            var health = GetHealth(other.gameObject);
            health?.UpdateHealth(damage, DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(this.transform.position), DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(GameState.MainCamera.transform.position), DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(piranha.ModelTrans.position), DamageType.piranhaBiteDamage);
            if(health != null) piranha.BiteGainAmmo();
        } 
    }
}
