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
        fire2timer = waitBeforeDamage + biteDamageDuration + 0.1f;
        yield return new WaitForSeconds(waitBeforeDamage);
        biteColliderGO.SetActive(true);
        yield return new WaitForSeconds(biteDamageDuration);
        biteColliderGO.SetActive(false);
    }
}
