using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyOnca : EnemyBiter
{
    private SkinnedMeshRenderer skinnedMesh;
    private Material normalMat;
    [SerializeField] private Material transMat;
    private float agentAngleSpeed;
    [Range(0,100)][SerializeField] private int invisibleChance;
    [SerializeField] private Vector2 transCooldown;
    private float transCooldownTimer;
    [SerializeField] private Vector2 transRNGDuration;
    private float transDurationTimer;
    private bool isInvisible;

    protected override void Start()
    {
        base.Start();
        agentAngleSpeed = agent.angularSpeed;
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        normalMat = skinnedMesh.material;
    }
    protected override void Update()
    {
        base.Update();
        transCooldownTimer -= Time.deltaTime;
        transDurationTimer -= Time.deltaTime;
    }

    protected override void AsyncUpdateIA()
    {
        var rng = Random.Range(0,100);
        if(!isInvisible && invisibleChance >= rng)
        {
            if(transCooldownTimer < 0) SetInvisibility(true);
            return;
        }
        else if(isInvisible && transDurationTimer < 0)
        {
            SetInvisibility(false);
        }
        base.AsyncUpdateIA();
    }

    public override void OnDamage()
    {
        base.OnDamage();
        SetInvisibility(false);
    }

    public virtual void SetInvisibility(bool flag)
    {
        isInvisible = flag;
        if(flag)
        {
            transDurationTimer = Random.Range(transRNGDuration.x, transRNGDuration.y);
            skinnedMesh.material = transMat;
        }
        else
        {
            transDurationTimer = 0;
            transCooldownTimer = Random.Range(transCooldown.x, transCooldown.y);
            skinnedMesh.material = normalMat;
        }
    }

    override protected IEnumerator BiteCourotine()
    {
        isBiting = true;
        agent.speed = runSpeed;
        agent.angularSpeed = agentAngleSpeed * 3;
        yield return new WaitForSeconds(biteWaitStart);
        biteCollider.SetActive(true);
        yield return new WaitForSeconds(biteTime);
        biteCollider.SetActive(false);
        agent.speed = lowSpeed;
        yield return new WaitForSeconds(biteCooldownEnd);
        isBiting = false;
        biteCourotine = null;
        agent.speed = walkingSpeed;
        agent.angularSpeed = agentAngleSpeed;
    }
}
