using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBiter : EnemyIA
{
    [SerializeField] private GameObject biteCollider;
    [SerializeField] private float biteWaitStart = 0.1f;
    [SerializeField] private float biteTime = 0.1f;
    [SerializeField] private float biteCooldownEnd = 0.2f;
    [SerializeField] bool doesShoot = true;
    private Gun gun;
    protected bool isBiting;
    private Coroutine biteCourotine;
    protected override void Start()
    {
        base.Start();
        gun = GetComponentInChildren<Gun>();
        biteCollider.SetActive(false);
    }
    protected override void Update() 
    {
        base.Update();
    }

    float shootRNG;
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        shootRNG = Random.Range(0f,1f);
        Debug.Log("RNG " + shootRNG);

        if(!isBiting)
        {
            if(distance <= minPlayerDistance) BitePlayer();
            else if(doesShoot && shootChance >= shootRNG && gun.LoadedAmmo > 0 && gun.Fire1Timer < 0) ShootAtPlayer();
            else GoToPlayer();
        }
    }

    protected virtual void BitePlayer()
    {
        if(biteCourotine == null) biteCourotine = StartCoroutine(BiteCourotine());
    }

    protected virtual void ShootAtPlayer()
    {
        Debug.Log("shoot player " + shootChance +" >= " + shootRNG);
        StopMoving();
        //TurnToPlayer();
        var playerNoYPos = player.position;
        playerNoYPos.y = this.transform.position.y;
        var distance = Vector3.Distance(this.transform.position, playerNoYPos);

        var missRotation = new Vector3(Random.Range(-1f, 1f),Random.Range(-1f, 1f), 0);
        var targetPos = player.position;
        targetPos += missRotation;
        if(gun is AcidGun) targetPos.y += distance / 2;
        gun.PrimaryFire();
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
