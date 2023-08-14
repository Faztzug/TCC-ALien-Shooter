using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEletric : EnemyBiter
{
    private float biteDamage;
    [SerializeField] private EletricVFXManager eletricVFX;
    private Rigidbody playerRgbd;

    protected override void Start() 
    {
        base.Start();
        playerRgbd = player.GetComponentInChildren<Rigidbody>();
        biteDamage = GetComponentInChildren<EnemyTouch>(includeInactive: true).Damage;
    }

    protected override void Update() 
    {
        base.Update();
        if(!isBiting) eletricVFX.TurnOffLAser();
        else eletricVFX.SetLaser(eletricVFX.transform.position, player.position + (playerRgbd != null ? playerRgbd.centerOfMass : Vector3.zero));
    }

    protected override IEnumerator BiteCourotine()
    {
        return base.BiteCourotine();
    }
    public override void OnDamage(DamageType damageType)
    {
        base.OnDamage(damageType);
        if(damageType == DamageType.piranhaBiteDamage) 
        {
            GameState.PlayerTransform.GetComponentInChildren<PlayerShieldHealth>().PierciShieldDamage(biteDamage);
        }
    }
}
