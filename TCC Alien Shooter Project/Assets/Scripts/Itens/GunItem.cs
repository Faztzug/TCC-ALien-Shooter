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
            var gunsManager = GameState.PlayerTransform.GetComponentInChildren<GunManager>();
            gunsManager.UpdateAvaibleGuns();
            gunsManager.SetSelectedGun(ammoType);
            base.CollectItem(obj);
            DestroyItem();
            return;
        }
        base.CollectItem(obj);
    }
}
