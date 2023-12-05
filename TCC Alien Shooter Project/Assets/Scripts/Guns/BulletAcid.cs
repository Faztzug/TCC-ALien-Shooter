using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAcid : Bullet
{
    [SerializeField] private GameObject acidPoolGO;
    [SerializeField] private GameObject explosionGO;

    public override Health BulletHit(GameObject collision, bool isTrigger = false)
    {
        if(!hit)
        {
            if(acidPoolGO) GameObject.Instantiate(acidPoolGO, transform.position, Quaternion.identity, null);
            if(explosionGO) GameObject.Instantiate(explosionGO, transform.position, Quaternion.identity, null);
        }
        return base.BulletHit(collision, isTrigger);
    }

}
