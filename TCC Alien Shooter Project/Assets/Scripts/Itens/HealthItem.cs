using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        if(obj.TryGetComponent<PlayerShieldHealth>(out PlayerShieldHealth hp))
        {
            if(hp.IsMaxHealth) return;
            var porcent = ammount / 100f;
            var value = hp.maxHealth * porcent;
            hp.GainHealth(value);
            DestroyItem();
        }
    }
}
