using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidGun : Gun
{
    [SerializeField] private Bullet explosiveBullet;
    protected override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(bulletPrefab);
    }

    protected override void SecondaryFire()
    {
        base.SecondaryFire();
        Shooting(explosiveBullet);
    }
}
