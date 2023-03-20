using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    public override void CollectItem(Collider info)
    {
        base.CollectItem(info);
        if(info.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth hp))
        {
            if(hp.IsMaxHealth) return;
            var porcent = ammount / 100f;
            var value = hp.maxHealth * porcent;
            hp.UpdateHealth(value);
            DestroyItem();
        }
    }
}
