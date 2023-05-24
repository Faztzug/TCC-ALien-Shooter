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
    protected Coroutine biteCourotine;
    [SerializeField] protected Sound melleSound;
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
            if(distance <= minPlayerDistance) 
            {
                GoToPlayerDirect();
                BitePlayer();
            }
            else if(doesShoot && shootChance >= shootRNG && gun.LoadedAmmo > 0 
            && gun.Fire1Timer < 0 && inFireRange) 
            {
                StopMoving();
                PrimaryFire();
            }
            else if(inWalkRange) 
            {
                GoToPlayerOffset();
            }
        }
    }

    protected virtual void BitePlayer()
    {
        if(biteCourotine == null) biteCourotine = StartCoroutine(BiteCourotine());
    }

    protected virtual IEnumerator BiteCourotine()
    {
        melleSound?.PlayOn(audioSource);
        anim.SetTrigger("Melle");
        isBiting = true;
        yield return new WaitForSeconds(biteWaitStart);
        GoToPlayerDirect();
        biteCollider.SetActive(true);
        yield return new WaitForSeconds(biteTime);
        biteCollider.SetActive(false);
        yield return new WaitForSeconds(biteCooldownEnd);
        isBiting = false;
        biteCourotine = null;
    }
}
