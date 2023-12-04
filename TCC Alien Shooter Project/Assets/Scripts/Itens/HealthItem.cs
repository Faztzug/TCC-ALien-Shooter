using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    /*[SerializeField] protected string TextFullHealth = "Vida Cheia";
    public override string InteractText
    {
        get
        {
            if (GameState.PlayerTransform.GetComponent<PlayerShieldHealth>().IsMaxShield) return TextFullHealth;
            else return base.InteractText;
        }
    }*/
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        if(obj.TryGetComponent<PlayerShieldHealth>(out PlayerShieldHealth hp))
        {
            if(hp.IsMaxHealth) return;
            var porcent = ammount / 100f;
            var value = hp.maxHealth * porcent;
            hp.GainHealth(value);
            GameState.mainCanvas.InstantiateVFX(GameState.mainCanvas.healthRecoverVFX);
            DestroyItem();
        }
    }
}
