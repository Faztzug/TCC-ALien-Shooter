using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidGun : Gun
{
    public override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(primaryFireData);
    }

    public override void SecondaryFire()
    {
        base.SecondaryFire();
        Shooting(secondaryFireData);
    }
}
