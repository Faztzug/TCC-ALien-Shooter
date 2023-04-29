using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidGun : Gun
{
    [SerializeField] private Bullet explosiveBullet;
    public override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(bulletPrefab);
    }

    public override void SecondaryFire()
    {
        base.SecondaryFire();
        Shooting(explosiveBullet);
    }
}
