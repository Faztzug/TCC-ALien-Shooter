using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
    /*[SerializeField] protected string TextFullShield = "Escudo Cheio";
    public override string InteractText
    {
        get
        {
            if (GameState.PlayerTransform.GetComponent<PlayerShieldHealth>().IsMaxShield) return TextFullShield;
            else return base.InteractText;
        }
    }*/

    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        if (obj.TryGetComponent<PlayerShieldHealth>(out PlayerShieldHealth hp))
        {
            if (hp.IsMaxShield) return;
            var porcent = ammount / 100f;
            var value = hp.MaxShield * porcent;
            hp.RecoverShield(value);
            DestroyItem();
        }
    }
}
