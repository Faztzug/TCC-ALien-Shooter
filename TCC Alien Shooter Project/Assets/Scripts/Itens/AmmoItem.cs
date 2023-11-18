using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] protected GunType ammoType;
    [SerializeField] private int[] ammoRange = new int[2];
    /*[SerializeField] protected string TextFullAmmo = "Munição Cheia";
    public override string InteractText
    {
        get
        {
            var gun = GameState.gunManager.AvaibleGuns.Find(g => g.gunType == ammoType);
            if (gun.isFullAmmo) return TextFullAmmo;
            else return base.InteractText;
        }
    }*/
    protected override void Start() 
    {
        base.Start();
        if(ammoRange.Length >= 2) ammount = Random.Range(ammoRange[0], ammoRange[1]+1);
    }
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        var guns = obj.GetComponentsInChildren<Gun>(true);
        foreach (var gun in guns) if(gun.gunType == ammoType)
        {
            gun.GainAmmo(ammount, this);
        }
    }
}
