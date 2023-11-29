using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemyHumanoid : EnemyHumanoid
{
    protected Health health;
    protected DamageModified damageImunity;
    [SerializeField] [Range(0, 1)] protected float biteChance = 1f; 
    protected override void Start()
    {
        base.Start();
        damageImunity = new DamageModified(DamageType.AnyDamage, 0f);
        health = GetComponentInChildren<Health>();
    }
    protected override void AsyncUpdateIA()
    {
        if((distance >= findPlayerDistance || !inFireRange) & !health.damageModifiers.Contains(damageImunity))
        {
            health.damageModifiers.Add(damageImunity);
            anim.SetBool("crouching", true);
            keepFiringTimer = 0;
            //Debug.Log("ADD Imunity");
        }
        else if(distance < findPlayerDistance & inFireRange)
        {
            health.damageModifiers.RemoveAll(d => d.damageType == damageImunity.damageType);
            //Debug.Log("REMOVE Imunity");
            anim.SetBool("crouching", false);
            base.AsyncUpdateIA();
        }

        if(distance <= 1f)
        {
            if(gun is PiranhaGun & gun.primaryFireData.fireTimer <= 0)
            {
                BitePlayer();
            }
        }
    }
}