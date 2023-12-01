using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))] 
public class EnemyIA : MonoBehaviour
{
    [SerializeField] [Range(0,1)] protected float[] updateRateRNG = new float[2];
    [Range(0, 1)] protected float updateRate;
    protected float advanceUpdate = 0f;
    [HideInInspector] public Transform player => GameState.PlayerTransform;
    [SerializeField] protected Vector2 playerOffsetXRNG;
    [SerializeField] protected Vector3 playerOffsetGoTo;
    protected NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    protected Rigidbody rgbd;
    [SerializeField] protected float lowSpeed = 3f;
    [SerializeField] protected float walkingSpeed = 5f;
    [SerializeField] protected float runSpeed = 8f;
    [SerializeField] protected float shootingDistance = 100f;
    [SerializeField] protected float findPlayerDistance = 100f;
    [SerializeField] protected float minPlayerDistance = 10f;
    protected float distance => Vector3.Distance(player.position, this.transform.position);
    protected Animator anim;
    [Range(0,100)] [SerializeField] protected float shootAim = 90;
    [Range(0f,0.1f)] [SerializeField] protected float shootChance = 0.08f;
    [HideInInspector] public bool alive = true;
    protected Gun gun;
    protected Vector3 missTargetPos = new Vector3();
    //protected bool DoesContinuousFire => gun is PiranhaGun;
    [SerializeField] protected Transform aimTransform;
    [SerializeField] private float aimRotationSpeed = 5f;
    protected Vector3 targetPos;
    protected Transform newTargetTrans;

    protected bool inFireRange => distance <= shootingDistance;
    protected bool inWalkRange => distance <= findPlayerDistance && distance >= minPlayerDistance;
    protected AudioSource audioSource;

    [SerializeField] private Color gizmoColor = new Color(0.7f, 0.75f, 0.2f, 0.2f);
    [HideInInspector] public bool countsToBodyCount = true;
    protected virtual bool allowRenewTargetPos => true;

    protected virtual void Start() 
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        agent = GetComponent<NavMeshAgent>();
        rgbd = GetComponent<Rigidbody>();
        rgbd.maxAngularVelocity = 0;
        anim = GetComponentInChildren<Animator>();
        gun = GetComponentInChildren<Gun>();
        if(gun != null) gun.aimTransform = aimTransform;
        newTargetTrans = GameState.PlayerMiddleT;
        targetPos = newTargetTrans.position;
        agent.speed = walkingSpeed;
        playerOffsetGoTo.x = Random.Range(playerOffsetXRNG.x, playerOffsetXRNG.y);
        audioSource = GetComponentInChildren<AudioSource>();
        
        StartCoroutine(CourotineAsyncUpdateIA());
        if(countsToBodyCount) GameState.nEnemies++;
    }

    protected virtual void Update() 
    {
        if (alive) targetPos = Vector3.Lerp(targetPos, newTargetTrans.position, aimRotationSpeed * Time.deltaTime);
        else
        {
            StopMoving();
            GetComponentInChildren<Gun>(true)?.TurnOffLasers();
            this.enabled = false;
            return;
        }
        // var directionPlayer = targetPos - aimTransform.position;

        // var newDirection = Vector3.RotateTowards(aimTransform.forward, directionPlayer, aimRotationSpeed, 0);
        // aimTransform.rotation = Quaternion.LookRotation(newDirection);
        if (gun != null)
        {
            aimTransform.LookAt(targetPos);
            gun.transform.LookAt(targetPos);
        }
    }

    protected IEnumerator CourotineAsyncUpdateIA()
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        updateRate = Mathf.Clamp(updateRate - advanceUpdate, 0, updateRateRNG[1]);
        advanceUpdate = 0f;

        yield return new WaitForSeconds(updateRate);

        rgbd.velocity = Vector3.zero;

        AsyncUpdateIA();

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
        StopMoving();
        if (gameObject.TryGetComponent<EnemyDrop>(out EnemyDrop drop))
        {
            drop.Drop();
        }
        if(anim == null) GameObject.Destroy(this.gameObject);
        if(anim != null) anim.SetBool("Death", true);
        if(anim != null) anim.SetTrigger("death");

        if(agent.isOnNavMesh) agent.SetDestination(transform.position);
        if(agent.isOnNavMesh) agent.isStopped = true;
        agent.enabled = false;
        rgbd.velocity = Vector3.zero;
        rgbd.useGravity = false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            transform.DOMove(hit.point, 1f).SetEase(Ease.InSine);
        }

        if(!alive) return;
        if(countsToBodyCount) GameState.nKillEnemies++;
        alive = false;
        this.StopAllCoroutines();
    }
    public virtual void OnDamage(DamageType damageType)
    {
        if(distance >= findPlayerDistance & !inFireRange) GoToPlayerDirect(ignoreFindDistance: true);
    }

    public void GoToPlayerDirect(bool ignoreFindDistance = false)
    {
        if(agent.isOnNavMesh)
        {
            if((ignoreFindDistance || distance < findPlayerDistance) && IsPlayerAlive()) //ignores min distance
            {
                var playerFlatPos = player.position;
                playerFlatPos.y = transform.position.y;
                var directionPlayer = playerFlatPos - transform.position;

                var thisPos = transform.position;
                agent.isStopped = false;
                var sucess = agent.SetDestination(player.position + directionPlayer * 0.5f);
            }
            else 
            {
                var pos = transform.position;
                agent.SetDestination(pos);
                rgbd.velocity = Vector3.zero;
                rgbd.angularVelocity = Vector3.zero;
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

    public void StopMoving()
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
    private void ShootAtPlayer(GunFireStruct fireMode)
    {
        if (!allowRenewTargetPos) return;

        var playerNoYPos = player.position;
        playerNoYPos.y = this.transform.position.y;
        var distanceFlat = Vector3.Distance(this.transform.position, playerNoYPos);

        var rngValue = (1f / shootAim) * distanceFlat * 2f;
        var rngMissTarget = new Vector3(Random.Range(-rngValue, rngValue),Random.Range(-rngValue, rngValue), Random.Range(-rngValue, rngValue));

        newTargetTrans = GameState.playerRandomBodyPart;
        targetPos = newTargetTrans.position;

        if (!fireMode.continuosFire && Vector3.Distance(missTargetPos, Vector3.zero) < 0.01f) missTargetPos = rngMissTarget;
        else missTargetPos = Vector3.Lerp(missTargetPos, rngMissTarget, Time.deltaTime);
        targetPos += missTargetPos;

        var isPlayerAbove = player.position.y >= transform.position.y;
        if(gun is AcidGun) 
        { 
            var plusY = new Vector3(0, distance / 16f, 0);
            targetPos += plusY;
        }

        if(!fireMode.continuosFire) targetPos = Vector3.Lerp(targetPos, newTargetTrans.position, aimRotationSpeed * Time.deltaTime);

        gun.enemyTarget = targetPos;
    }

    protected virtual void PrimaryFire()
    {
        //if(gun.primaryFireData.continuosFire | gun is PiranhaGun | gun is EletricGun)
        //{
        //    if (gun.IsACloseObstacleOnFire()) return;
        //}
        ShootAtPlayer(gun.primaryFireData);
        gun.PrimaryFire();
        anim.SetTrigger("Fire");
    }
    protected virtual void SecondaryFire()
    {
        ShootAtPlayer(gun.secondaryFireData);
        gun.SecondaryFire();
        anim.SetTrigger("Fire");
    }
    //protected virtual void HoldSecondaryFire()
    //{
    //    ShootAtPlayer();
    //    gun.SecondaryFire();
    //}

    private void OnDrawGizmos() 
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, findPlayerDistance);
    }
}
