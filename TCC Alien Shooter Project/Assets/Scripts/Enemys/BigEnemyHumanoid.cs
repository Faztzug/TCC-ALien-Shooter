using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemyHumanoid : EnemyHumanoid
{
    protected Health health;
    protected DamageModified damageImunity;
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
            //Debug.Log("ADD Imunity");
        }
        else if(distance < findPlayerDistance & inFireRange)
        {
            health.damageModifiers.RemoveAll(d => d.damageType == damageImunity.damageType);
            //Debug.Log("REMOVE Imunity");
            anim.SetBool("crouching", false);
            base.AsyncUpdateIA();
        }
    }
}