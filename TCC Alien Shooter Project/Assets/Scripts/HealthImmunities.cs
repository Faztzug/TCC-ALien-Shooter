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
    AnyDamage,
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

    public override void BleedVFX(Vector3 position, DamageType damageType, bool isContinuos = false)
    {
        if(damageOnlyFrom != null && damageOnlyFrom.Count > 0)
        {
            if(!damageOnlyFrom.Contains(damageType)) return;
        }
        base.BleedVFX(position, damageType, isContinuos);
    }
}
