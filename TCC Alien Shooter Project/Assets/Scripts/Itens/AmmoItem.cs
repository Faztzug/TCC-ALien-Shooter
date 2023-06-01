using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] protected GunType ammoType;
    [SerializeField] private int[] ammoRange = new int[2];
    protected override void Start() 
    {
        base.Start();
        ammount = Random.Range(ammoRange[0], ammoRange[1]+1);
    }
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        var guns = obj.GetComponentsInChildren<Gun>(true);
        foreach (var gun in guns) if(gun.gunType == ammoType)
        {
            gun.GainAmmo(ammount, this);
            Debug.Log("collecting ammo: " + ammoType + " found gun? " + guns.Length);
        }
    }
}
