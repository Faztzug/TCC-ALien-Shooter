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
    [SerializeField] protected Vector2 playerOffsetXRNG;
    [SerializeField] protected Vector3 playerOffsetGoTo;
    protected NavMeshAgent agent;
    protected Rigidbody rgbd;
    [SerializeField] protected float lowSpeed = 3f;
    [SerializeField] protected float walkingSpeed = 5f;
    [SerializeField] protected float runSpeed = 8f;
    [SerializeField] protected float shootingDistance = 100f;
    [SerializeField] protected float findPlayerDistance = 100f;
    [SerializeField] protected float minPlayerDistance = 10f;
    [Range(0,1)] protected float updateRate;
    protected float distance => Vector3.Distance(player.position, this.transform.position);
    protected Animator anim;
    [Range(0,100)] [SerializeField] protected float shootAim = 90;
    [Range(0f,0.1f)] [SerializeField] protected float shootChance = 0.08f;
    [HideInInspector] public bool alive = true;
    [SerializeField] protected int damageTauntAsync = 3;
    protected int tauntTimerAsync;
    protected Gun gun;
    protected Vector3 missTargetPos = new Vector3();
    protected bool doesContinuousFire => gun is PiranhaGun;
    [SerializeField] protected Transform aimTransform;
    [SerializeField] private float aimRotationSpeed = 5f;
    protected Vector3 targetPos;
    protected Transform newTargetTrans;

    protected bool inFireRange => distance <= shootingDistance;
    protected bool inWalkRange => distance <= findPlayerDistance && distance >= minPlayerDistance;
    protected AudioSource audioSource;

    [SerializeField] private Color gizmoColor = Color.yellow;

    protected virtual void Start() 
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        agent = GetComponent<NavMeshAgent>();
        rgbd = GetComponent<Rigidbody>();
        rgbd.maxAngularVelocity = 0;
        tauntTimerAsync = damageTauntAsync * 3;
        anim = GetComponentInChildren<Animator>();
        gun = GetComponentInChildren<Gun>();
        if(gun != null) gun.aimTransform = aimTransform;
        targetPos = player.position;
        newTargetTrans = player;
        agent.speed = walkingSpeed;
        playerOffsetGoTo.x = Random.Range(playerOffsetXRNG.x, playerOffsetXRNG.y);
        audioSource = GetComponentInChildren<AudioSource>();
        
        StartCoroutine(CourotineAsyncUpdateIA());
    }

    protected virtual void Update() 
    {
        targetPos = Vector3.Lerp(targetPos, newTargetTrans.position, aimRotationSpeed * Time.deltaTime);
        // var directionPlayer = targetPos - aimTransform.position;
        
        // var newDirection = Vector3.RotateTowards(aimTransform.forward, directionPlayer, aimRotationSpeed, 0);
        // aimTransform.rotation = Quaternion.LookRotation(newDirection);
        if(gun != null) aimTransform.LookAt(targetPos);
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
        if(gameObject.TryGetComponent<EnemyDrop>(out EnemyDrop drop))
        {
            drop.Drop();
        }
        if(anim == null) GameObject.Destroy(this.gameObject);
        if(anim != null) anim.SetBool("Death", true);
        if(anim != null) anim.SetTrigger("death");

        if(agent.isOnNavMesh) agent.SetDestination(transform.position);
        if(agent.isOnNavMesh) agent.isStopped = true;

        alive = false;

        // foreach (var collider in GetComponentsInChildren<Collider>())
        // {
        //     collider.enabled = false;
        // }
        // foreach (var script in GetComponentsInChildren<MonoBehaviour>(true))
        // {
        //     if(script == this) continue;
        //     if(script is Gun) (script as LaserVFXManager).TurnOffLAser();
        //     if(script is LaserVFXManager) (script as LaserVFXManager).TurnOffLAser();
        //     script.enabled = false;
        // }
        this.StopAllCoroutines();
    }
    public virtual void OnDamage()
    {
        GoToPlayerDirect(ignoreFindDistance: true);
    }

    protected void GoToPlayerDirect(bool ignoreFindDistance = false)
    {
        if(agent.isOnNavMesh)
        {
            if((ignoreFindDistance || distance < findPlayerDistance) && IsPlayerAlive()) //ignores min distance
            {
                var playerFlatPos = player.position;
                playerFlatPos.y = transform.position.y;
                var directionPlayer = playerFlatPos - transform.position;

                var thisPos = transform.position;
                agent.SetDestination(player.position + directionPlayer * 0.5f);
                agent.isStopped = false;
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
    protected void GoToPlayerOffset(bool goToOffset = true)
    {
        if(agent.isOnNavMesh)
        {
            if(distance > minPlayerDistance && distance < findPlayerDistance && IsPlayerAlive())
            {
                var offset = goToOffset ? (player.rotation * playerOffsetGoTo) : Vector3.zero;
                agent.SetDestination(player.position + offset);
                agent.isStopped = false;
                // if(Vector3.Distance(playerOffsetGoTo, Vector3.zero) > 0) 
                // {
                //     Debug.Log("going to player " + (player.rotation * playerOffsetGoTo));
                // }
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
        if(agent.isOnNavMesh) 
        {   
            agent.SetDestination(this.transform.position);
            agent.isStopped = true;
        }
    }

    protected bool IsMoving() => agent.isOnNavMesh && !agent.isStopped && Vector3.Distance(agent.destination, transform.position) > minPlayerDistance;

    protected void TurnToPlayer()
    {
        var playerFlatPos = player.position;
        playerFlatPos.y = transform.position.y;
        var directionPlayer = playerFlatPos - transform.position;
        //var newDirection = Vector3.RotateTowards(transform.forward, directionPlayer, 30, 0);

        transform.DORotate(directionPlayer, 1f, RotateMode.FastBeyond360);
    }
    protected float shootRNG;
    private void ShootAtPlayer()
    {
        //Debug.Log("shoot player " + shootChance +" >= " + shootRNG);
        //StopMoving();
        var playerNoYPos = player.position;
        playerNoYPos.y = this.transform.position.y;
        var distanceFlat = Vector3.Distance(this.transform.position, playerNoYPos);

        var rngValue = (1f / shootAim) * distance;
        var rngMissTarget = new Vector3(Random.Range(-rngValue, rngValue),Random.Range(-rngValue, rngValue), Random.Range(-rngValue, rngValue));
        newTargetTrans = GameState.playerRandomBodyPart;
        if(!doesContinuousFire && Vector3.Distance(missTargetPos, Vector3.zero) < 0.01f) missTargetPos = rngMissTarget;
        else missTargetPos = Vector3.Lerp(missTargetPos, rngMissTarget, Time.deltaTime);
        newTargetTrans.position += missTargetPos;
        var isPlayerAbove = player.position.y >= transform.position.y;
        if(gun is AcidGun) 
        { 
            var plusY = new Vector3(0, distance / 16f, 0);
            newTargetTrans.position += plusY;
        }

        if(!doesContinuousFire) targetPos = Vector3.Lerp(targetPos, newTargetTrans.position, aimRotationSpeed * Time.deltaTime);

        gun.enemyTarget = doesContinuousFire ? targetPos : newTargetTrans.position;
    }

    protected virtual void PrimaryFire()
    {
        if(gun.IsACloseObstacleOnFire()) return;
        ShootAtPlayer();
        gun.PrimaryFire();
        anim.SetTrigger("Fire");
    }
    protected virtual void SecondaryFire()
    {
        if(gun.IsACloseObstacleOnFire()) return;
        ShootAtPlayer();
        gun.SecondaryFire();
    }
    protected virtual void HoldSecondaryFire()
    {
        ShootAtPlayer();
        gun.HoldSencondaryFire();
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, findPlayerDistance);
    }
}
