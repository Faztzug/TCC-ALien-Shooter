using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Health
{
    [SerializeField] protected float maxShield = 1;
    [SerializeField] protected float curShield;
    [SerializeField] protected float shieldRegen = 1f;
    [SerializeField] protected float regenCooldown = 0.5f;
    private float regenTimer = 0f;

    public override void Start()
    {
        base.Start();
        curShield = maxShield;
    }

    public override void UpdateHealth(float value = 0)
    {
        if(curShield <= 0 || value > 0)
        {
            base.UpdateHealth(value);
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
