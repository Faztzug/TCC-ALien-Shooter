using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    NULL,
    piranhaBiteDamage,
    heatLaserDamage,
    acidDamage,
    eletricDamage,
}
public class HealthImmunities : Health
{
    public List<DamageType> damageOnlyFrom;

    public override void UpdateHealth(float value, DamageType damageType)
    {
        if(damageOnlyFrom != null && damageOnlyFrom.Count > 0)
        {
            if(!damageOnlyFrom.Contains(damageType)) return;
        }
        base.UpdateHealth(value, damageType);
    }
}
