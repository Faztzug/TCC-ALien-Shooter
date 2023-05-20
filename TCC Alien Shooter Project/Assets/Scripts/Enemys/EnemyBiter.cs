using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBiter : EnemyIA
{
    [SerializeField] protected GameObject biteCollider;
    [SerializeField] protected float biteWaitStart = 0.1f;
    [SerializeField] protected float biteTime = 0.1f;
    [SerializeField] protected float biteCooldownEnd = 0.2f;
    [SerializeField] protected bool doesShoot = true;
    protected bool isBiting;
    private Coroutine biteCourotine;
    protected override void Start()
    {
        base.Start();
        biteCollider.SetActive(false);
    }
    protected override void Update() 
    {
        base.Update();
    }

    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        shootRNG = Random.Range(0f,1f);

        if(!isBiting)
        {
            if(distance <= minPlayerDistance) BitePlayer();
            else if(doesShoot && shootChance >= shootRNG && gun.LoadedAmmo > 0 
            && gun.Fire1Timer < 0 && inFireRange) 
            {
                PrimaryFire();
            }
            else if(inWalkRange) GoToPlayer();
        }
    }

    protected virtual void BitePlayer()
    {
        if(biteCourotine == null) biteCourotine = StartCoroutine(BiteCourotine());
    }


    protected IEnumerator BiteCourotine()
    {
        isBiting = true;
        yield return new WaitForSeconds(biteWaitStart);
        biteCollider.SetActive(true);
        yield return new WaitForSeconds(biteTime);
        biteCollider.SetActive(false);
        yield return new WaitForSeconds(biteCooldownEnd);
        isBiting = false;
        biteCourotine = null;
    }
}
