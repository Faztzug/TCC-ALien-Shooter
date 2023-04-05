using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBiter : EnemyIA
{
    [SerializeField] private GameObject biteCollider;
    [SerializeField] private float biteWaitStart = 0.1f;
    [SerializeField] private float biteTime = 0.1f;
    [SerializeField] private float biteCooldownEnd = 0.2f;
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

        if(!isBiting)
        {
            if(distance <= minPlayerDistance) BitePlayer();
            else base.GoToPlayer();
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
