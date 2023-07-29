using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricGun : Gun
{
    private float chargingPower;
    public override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(primaryFireData);
    }

    public override void SecondaryFire()
    {
        base.SecondaryFire();
        Shooting(secondaryFireData);
        if(secondaryFireData.continuosFire) chargingPower += Time.deltaTime;
    }
}
