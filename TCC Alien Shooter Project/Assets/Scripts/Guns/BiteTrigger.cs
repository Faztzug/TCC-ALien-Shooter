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
            var dmg = damage;
            if(gameObject.CompareTag(Gun.kCrtiHitTag)) dmg *= 2;

            var health = GetHealth(other.gameObject);

            health?.UpdateHealth(dmg, DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(this.transform.position), DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(GameState.MainCamera.transform.position), DamageType.piranhaBiteDamage);
            health?.BleedVFX(other.ClosestPointOnBounds(piranha.ModelTrans.position), DamageType.piranhaBiteDamage);
            if(health != null) piranha.BiteGainAmmo();

            var hitVFX = piranha.primaryFireData.defaultHitVFX;
            if(hitVFX != null & health == null)
            {
                var go = GameObject.Instantiate(hitVFX, this.transform.position, piranha.ModelTrans.rotation, null);
                var duration = hitVFX.GetComponentInChildren<ParticleSystem>().main.duration;
                if (duration == 0) duration = 5f;
                GameObject.Destroy(go, duration);
            }

            if(piranha.primaryFireData.hitSound.clip != null)
            {
                GameState.InstantiateSound(piranha.primaryFireData.hitSound, this.transform.position);
            }
        } 
    }
}
