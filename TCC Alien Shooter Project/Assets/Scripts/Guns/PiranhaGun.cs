using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class PiranhaGun : Gun
{
    [SerializeField] BiteTrigger biteCollider;
    [SerializeField] float waitBeforeDamage;
    [SerializeField] float biteDamageDuration;
    [SerializeField] float biteAmmoGain = 1f;
    bool firing;
    [SerializeField] Transform modelTrans;
    [SerializeField] Transform biteEndTrans;
    private Vector3 modelStartLocalPos;
    private Vector3 biteEndLocalPos;

    protected override void Start()
    {
        base.Start();
        biteCollider.gameObject.SetActive(false);
        biteCollider.piranha = this;
        biteEndLocalPos = biteEndTrans.localPosition;
        modelStartLocalPos = modelTrans.localPosition;
    }


    public override void PrimaryFire()
    {
        base.PrimaryFire();
        BiteTask();
    }

    public override void HoldSencondaryFire()
    {
        base.HoldSencondaryFire();
        Shooting(DamageType.heatLaserDamage);
    }

    public void BiteGainAmmo()
    {
        GainAmmo(biteAmmoGain, null);
    }

    private async void BiteTask()
    {
        // Tween[] tweens = new Tween[]
        // {modelTrans.DOLocalMove(biteEndLocalPos, waitBeforeDamage + (biteDamageDuration / 2)).SetEase(Ease.InCubic)
        // ,modelTrans.DOLocalMove(modelStartLocalPos, biteDamageDuration).SetEase(Ease.OutCubic)};

        // DOTween.Sequence(tweens);

        // DOTween.Sequence(
        //     modelTrans.DOLocalMove(biteEndLocalPos, waitBeforeDamage + (biteDamageDuration / 2)).SetEase(Ease.InCubic))
        //     .Append(modelTrans.DOLocalMove(modelStartLocalPos, biteDamageDuration).SetEase(Ease.OutCubic));
            //.OnComplete(() => Debug.Log("Bite Tween Ended " + biteEndLocalPos.ToString() + " " + modelStartLocalPos.ToString()));

        modelTrans.DOLocalMove(biteEndLocalPos, waitBeforeDamage + (biteDamageDuration / 2)).SetEase(Ease.InCubic)
        .OnComplete(() => modelTrans.DOLocalMove(modelStartLocalPos, biteDamageDuration).SetEase(Ease.OutCubic));
        fire2timer = waitBeforeDamage + biteDamageDuration + 0.1f;
        await Task.Delay((int)(waitBeforeDamage * 1000));
        biteCollider.gameObject.SetActive(true);
        await Task.Delay((int)(biteDamageDuration * 1000));
        biteCollider.gameObject.SetActive(false);
    }
}
