using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHumanoid : EnemyIA
{
    protected bool isContinousFiring => doesContinuousFire && keepFiringTimer > 0;
    protected float runSpeed;
    [Range(0f,1f)] [SerializeField] protected float walkingFiringSpeed = 0.6f;
    [SerializeField] [Range(0f,10f)] protected float[] keepFiringTimeRNG = new float[2];
    protected float keepFiringTimer;
    protected override void Start()
    {
        base.Start();
        runSpeed = agent.speed;
    }
    protected override void Update()
    {
        base.Update();
        keepFiringTimer -= Time.deltaTime;
        if(isContinousFiring) HoldSecondaryFire();
        gun.enemyHoldingFire = isContinousFiring;
    }
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        shootRNG = Random.Range(0f,1f);

        if(!isContinousFiring && doesContinuousFire)
        {
            agent.speed = runSpeed * walkingFiringSpeed;
            if(shootChance >= shootRNG && gun.LoadedAmmo > 0 && gun.Fire2Timer < 0)
            {
                keepFiringTimer = Random.Range(keepFiringTimeRNG[0],keepFiringTimeRNG[1]);
                HoldSecondaryFire();
            } 

            if(distance > minPlayerDistance && distance < findPlayerDistance) GoToPlayer();
            else if(agent.isOnNavMesh) agent.isStopped = true;
        }
        else if(isContinousFiring && doesContinuousFire)
        {
            HoldSecondaryFire();
        }
        else if(doesContinuousFire)
        {

        }
    }
}
