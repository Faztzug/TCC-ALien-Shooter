using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Health
{
    [SerializeField] protected float maxShield = 1;
    protected float curShield;
    [SerializeField] protected float shieldRegen = 1f;
    [SerializeField] protected float regenCooldown = 2f;
    private float regenTimer = 0f;

    public override void Start()
    {
        base.Start();
        curShield = maxShield;
    }

    public void PierciShieldDamage(float value)
    {
        if(curShield > 0) health += value / 2;
        UpdateHealth(value, DamageType.NULL);
    }

    public override void UpdateHealth(float value, DamageType damageType)
    {
        if(curShield <= 0 || value > 0)
        {
            base.UpdateHealth(value, damageType);
        } 
        
        if(value < 0)
        {
            regenTimer = regenCooldown;
            UpdateShieldValue(value);
        }
    }

    public virtual void RecoverShield(float value)
    {
        UpdateShieldValue(value);
    }

    protected virtual void Update() 
    {
        if(regenTimer < 0) UpdateShieldValue(shieldRegen * Time.deltaTime);
        
        regenTimer = regenTimer - Time.deltaTime;
    }

    protected void UpdateShieldValue(float value) => curShield = Mathf.Clamp(curShield + value, 0, maxShield);
}
