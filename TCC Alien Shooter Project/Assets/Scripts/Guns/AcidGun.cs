using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidGun : Gun
{
    public override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(primaryFireData);
        if(IsAmmoEmpty()) GameState.gunManager.NextGun();
    }

    public override void SecondaryFire()
    {
        base.SecondaryFire();
        Shooting(secondaryFireData);
    }
}
