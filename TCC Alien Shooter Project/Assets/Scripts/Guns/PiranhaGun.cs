using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaGun : Gun
{
    [SerializeField] GameObject biteColliderGO;
    [SerializeField] float waitBeforeDamage;
    [SerializeField] float biteDamageDuration;
    bool firing;

    protected override void Start()
    {
        base.Start();
        biteColliderGO.SetActive(false);
    }


    protected override void PrimaryFire()
    {
        base.PrimaryFire();
        StartCoroutine(BiteCourotine());
    }

    protected override void HoldSencondaryFire()
    {
        base.HoldSencondaryFire();
        Shooting();
    }

    IEnumerator BiteCourotine()
    {
        yield return new WaitForSeconds(waitBeforeDamage);
        biteColliderGO.SetActive(true);
        yield return new WaitForSeconds(biteDamageDuration);
        biteColliderGO.SetActive(false);
    }
}
