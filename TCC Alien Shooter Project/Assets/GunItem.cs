using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : AmmoItem
{
    public override void CollectItem(GameObject obj)
    {
        if(!(GameState.SaveData.gunsColected?.Contains(ammoType) == true))
        {
            GameState.SaveData.gunsColected.Add(ammoType);
            GameState.SaveGameData();
            GameState.PlayerTransform.GetComponentInChildren<GunManager>().SetSelectedGun(ammoType);
            base.CollectItem(obj);
            DestroyItem();
            return;
        }
        base.CollectItem(obj);
    }
}
