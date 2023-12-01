using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnemyHumanoid : EnemyIA
{
    protected bool isContinousFiring => gun is PiranhaGun & keepFiringTimer > 0;
    [SerializeField] [Range(0f,10f)] protected float[] keepFiringTimeRNG = new float[2];
    protected float keepFiringTimer;
    [SerializeField] protected Rig rightArmRig;
    protected override bool allowRenewTargetPos => (gun is PiranhaGun & keepFiringTimer <= 0) || targetPos == Vector3.zero;


    protected override void Start()
    {
        base.Start();
        runSpeed = agent.speed;
    }
    protected override void Update()
    {
        base.Update();
        if(!alive) return;

        if(gun is PiranhaGun)
        {
            gun.enemyHoldingFire = isContinousFiring;
            if (isContinousFiring) SecondaryFire();
            keepFiringTimer -= Time.deltaTime;
        }
    }
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        shootRNG = Random.Range(0f,1f);
        anim.SetFloat("Movement", agent.velocity.magnitude);
        anim.SetBool("isRuning", false);

        if(gun is PiranhaGun)
        {
            if(distance <= 3f & gun.primaryFireData.fireTimer <= 0)
            {
                BitePlayer();
            }
            else if (!gun.IsACloseObstacleOnFire() & keepFiringTimer <= 0)
            {
                agent.speed = walkingSpeed;
                if (shootChance >= shootRNG && gun.LoadedAmmo > 0 && gun.secondaryFireData.fireTimer < 0 & inFireRange)
                {
                    keepFiringTimer = Random.Range(keepFiringTimeRNG[0], keepFiringTimeRNG[1]);
                    SecondaryFire();
                }

                if (inWalkRange) GoToPlayerOffset();
                else if (agent.isOnNavMesh & inFireRange) StopMoving();
            }
            else if(isContinousFiring & inFireRange)
            {
                agent.speed = lowSpeed;
            }
            else if (distance >= minPlayerDistance & distance <= findPlayerDistance & distance >= shootingDistance)
            {
                GoToPlayerOffset();
                agent.speed = runSpeed;
                anim.SetBool("isRuning", true);
            }
        }
    }

    protected void BitePlayer()
    {
        keepFiringTimer = 0;
        advanceUpdate = 0.1f;
        if (distance <= 1f) PrimaryFire();
        GoToPlayerDirect();
        agent.speed = runSpeed;
        anim.SetBool("isRuning", true);
    }
}
