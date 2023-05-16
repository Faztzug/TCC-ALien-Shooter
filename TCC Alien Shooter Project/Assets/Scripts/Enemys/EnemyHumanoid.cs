using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHumanoid : EnemyIA
{
    protected bool isContinousFiring => doesContinuousFire && keepFiringTimer > 0;
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
        anim.SetFloat("Movement", agent.velocity.magnitude);
        anim.SetBool("isRuning", false);

        if(!isContinousFiring && doesContinuousFire)
        {
            agent.speed = walkingSpeed;
            if(shootChance >= shootRNG && gun.LoadedAmmo > 0 && gun.Fire2Timer < 0 && inFireRange)
            {
                keepFiringTimer = Random.Range(keepFiringTimeRNG[0],keepFiringTimeRNG[1]);
                HoldSecondaryFire();
            } 

            if(inWalkRange) GoToPlayer();
            else if(agent.isOnNavMesh) agent.isStopped = true;
        }
        else if(isContinousFiring && doesContinuousFire && distance <= shootingDistance)
        {
            HoldSecondaryFire();
            agent.speed = lowSpeed;
        }
        else if(distance >= minPlayerDistance && distance <= findPlayerDistance && distance >= shootingDistance)
        {
            GoToPlayer();
            agent.speed = runSpeed;
            anim.SetBool("isRuning", true);
        }
    }
}
