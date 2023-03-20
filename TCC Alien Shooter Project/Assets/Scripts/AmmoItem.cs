using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] private int[] ammoRange = new int[2];
    private void Start() 
    {
        ammount = Random.Range(ammoRange[0], ammoRange[1]+1);
    }
    public override void CollectItem(Collider info)
    {
        base.CollectItem(info);
        if(info.gameObject.TryGetComponent<Gun>(out Gun gun))
        {
            gun.GainAmmo(ammount,this);
        }
    }
}
