using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] private GunType ammoType;
    [SerializeField] private int[] ammoRange = new int[2];
    protected override void Start() 
    {
        base.Start();
        ammount = Random.Range(ammoRange[0], ammoRange[1]+1);
    }
    public override void CollectItem(Collider info)
    {
        base.CollectItem(info);
        var guns = info.GetComponentsInChildren<Gun>(true);
        foreach (var gun in guns) if(gun.gunType == ammoType) gun.GainAmmo(ammount, this);
        Debug.Log("collecting ammo: " + ammoType + " found gun? " + guns.Length);
    }
}
