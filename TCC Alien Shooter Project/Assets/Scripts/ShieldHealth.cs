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
    public Sound[] shieldDamageSounds;

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
            if(damageSoundTimer < 0 && shieldDamageSounds.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, shieldDamageSounds.Length);
                shieldDamageSounds[index]?.PlayOn(audioSource);
                damageSoundTimer = 1f;
                Debug.Log("Shield dmaage Sound " + name);
            }
            regenTimer = regenCooldown;
            UpdateShieldValue(value);
            if(health <= 0) DestroyCharacter();
        }
    }

    public virtual void RecoverShield(float value)
    {
        UpdateShieldValue(value);
    }

    protected override void Update() 
    {
        base.Update();
        if(CurHealth <= 0) return;
        if(regenTimer < 0) UpdateShieldValue(shieldRegen * Time.deltaTime);
        
        regenTimer = regenTimer - Time.deltaTime;
    }

    protected void UpdateShieldValue(float value) => curShield = Mathf.Clamp(curShield + value, 0, maxShield);
}
