using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))] 
public class EnemyIA : MonoBehaviour
{
    [SerializeField] [Range(0,1)] protected float[] updateRateRNG = new float[2];
    [HideInInspector] public Transform player => GameState.PlayerTransform;
    protected NavMeshAgent agent;
    protected Rigidbody rgbd;
    [SerializeField] protected float shootingDistance = 100f;
    [SerializeField] protected float findPlayerDistance = 100f;
    [SerializeField] protected float minPlayerDistance = 10f;
    [Range(0,1)] protected float updateRate;
    protected float distance => Vector3.Distance(player.position, this.transform.position);
    protected Animator anim;
    [Range(0f,0.1f)] [SerializeField] protected float shootChance = 0.8f;
    [HideInInspector] public bool alive = true;
    [SerializeField] protected int damageTauntAsync = 3;
    protected int tauntTimerAsync;

    protected virtual void Start() 
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        agent = GetComponent<NavMeshAgent>();
        rgbd = GetComponent<Rigidbody>();
        rgbd.maxAngularVelocity = 0;
        tauntTimerAsync = damageTauntAsync * 3;
        anim = GetComponent<Animator>();
        
        StartCoroutine(CourotineAsyncUpdateIA());
    }

    protected virtual void Update() 
    {
        
    }

    protected IEnumerator CourotineAsyncUpdateIA()
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);

        yield return new WaitForSeconds(updateRate);

        rgbd.velocity = Vector3.zero;

        if(tauntTimerAsync >= 0)
        {
            tauntTimerAsync--;
            //Debug.Log("Current Taunt = " + tauntTimerAsync + name);
        }
        else  AsyncUpdateIA();

        StartCoroutine(CourotineAsyncUpdateIA());
    }

    protected virtual void AsyncUpdateIA()
    {
        
    }
    protected bool IsPlayerAlive()
    {
        if(player != null && player.gameObject.activeSelf && GameState.IsPlayerDead == false) return true;
        else return false;
    }
    public virtual void EnemyDeath()
    {
        if(anim == null) GameObject.Destroy(this.gameObject);
        if(anim != null) anim.SetTrigger("Die");

        if(agent.isOnNavMesh) agent.SetDestination(transform.position);

        alive = false;

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        foreach (var script in GetComponentsInChildren<MonoBehaviour>())
        {
            if(script == this) continue;
            script.enabled = false;
        }
        this.StopAllCoroutines();
    }
    public virtual void Taunt()
    {
        tauntTimerAsync = damageTauntAsync;
    }
    public virtual void UpdateHealth(float health, float maxHealth)
    {

    }

    protected void GoToPlayer()
    {
        if(agent.isOnNavMesh)
        {
            if(distance > minPlayerDistance && distance < findPlayerDistance && IsPlayerAlive())
            {
                agent.SetDestination(player.position);
                agent.isStopped = false;
                //Debug.Log("going to player");
            }
            else 
            {
                var pos = transform.position;
                agent.SetDestination(pos);
                rgbd.velocity = Vector3.zero;
                rgbd.angularVelocity = Vector3.zero;
                //Debug.Log("not going to player");
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " OUT OF NAV MESH!");
        }
    }

    protected void StopMoving()
    {
        if(agent.isOnNavMesh) agent.SetDestination(this.transform.position);
        agent.isStopped = true;
    }

    protected void TurnToPlayer()
    {
        var playerFlatPos = player.position;
        playerFlatPos.y = transform.position.y;
        var directionPlayer = playerFlatPos - transform.position;
        //var newDirection = Vector3.RotateTowards(transform.forward, directionPlayer, 30, 0);

        transform.DORotate(directionPlayer, 1f, RotateMode.FastBeyond360);
    }
}
