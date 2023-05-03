using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PiranhaGun : Gun
{
    [SerializeField] BiteTrigger biteCollider;
    [SerializeField] float waitBeforeDamage;
    [SerializeField] float biteDamageDuration;
    [SerializeField] float biteAmmoGain = 1f;
    bool firing;

    protected override void Start()
    {
        base.Start();
        biteCollider.gameObject.SetActive(false);
        biteCollider.piranha = this;
    }


    public override void PrimaryFire()
    {
        base.PrimaryFire();
        BiteTask();
    }

    public override void HoldSencondaryFire()
    {
        base.HoldSencondaryFire();
        Shooting();
    }

    public void BiteGainAmmo()
    {
        GainAmmo(biteAmmoGain, null);
    }

    private async void BiteTask()
    {
        fire2timer = waitBeforeDamage + biteDamageDuration + 0.1f;
        await Task.Delay((int)(waitBeforeDamage * 1000));
        biteCollider.gameObject.SetActive(true);
        await Task.Delay((int)(biteDamageDuration * 1000));
        biteCollider.gameObject.SetActive(false);
    }
}
