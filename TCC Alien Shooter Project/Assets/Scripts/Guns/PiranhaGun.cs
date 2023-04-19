using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
        BiteTask();
    }

    protected override void HoldSencondaryFire()
    {
        base.HoldSencondaryFire();
        Shooting();
    }

    private async void BiteTask()
    {
        fire2timer = waitBeforeDamage + biteDamageDuration + 0.1f;
        await Task.Delay((int)(waitBeforeDamage * 1000));
        biteColliderGO.SetActive(true);
        await Task.Delay((int)(biteDamageDuration * 1000));
        biteColliderGO.SetActive(false);
    }
}
