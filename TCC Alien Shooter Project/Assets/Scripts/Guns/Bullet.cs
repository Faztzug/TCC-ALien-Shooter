using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public DamageType damageType;
    public bool isTraveling {private set; get;}
    private MeshRenderer meshRenderer;
    public string bulletType;
    [SerializeField] private Vector3 moveVector;
    private float speed;
    private float gravity;
    [SerializeField] private Rigidbody rgbd;
    public float damage;
    private Vector3 move;
    private bool setedVelocity = false;
    public float headShootMultiplier = 5f;
    [HideInInspector] public bool hit = false;
    [SerializeField] private Sound headShootSound;
    TrailRenderer[] trailRenderers = new TrailRenderer[]{};

    void Start()
    {
        speed = moveVector.z;
        gravity = moveVector.y;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
        
        setedVelocity = false;
    }

    void Update()
    {
        if(!setedVelocity)
        {
            move = rgbd.velocity + transform.forward * speed;
            setedVelocity = true;
        }
        
        move.y += gravity * Time.deltaTime;
        rgbd.velocity = move;

        gravity += moveVector.y * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        
        // if(collisionInfo.collider.gameObject.CompareTag("EnemyHead"))
        // {
        //     Debug.Log("HEADSHOT!" + collisionInfo.collider.gameObject);
        //     damage = damage * headShootMultiplier;
        //     //play sound
        // }
        if(collisionInfo.rigidbody?.gameObject != null) BulletHit(collisionInfo.rigidbody.gameObject);
        else BulletHit(collisionInfo.gameObject);
    }
    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.attachedRigidbody?.gameObject != null) BulletHit(other.attachedRigidbody.gameObject, true);
    //     else BulletHit(other.gameObject, true);
    // }
    public virtual void BulletHit(GameObject collision, bool isTrigger = false)
    {
        if(!isTrigger)
        {
            rgbd.velocity = Vector3.zero;
            rgbd.constraints = RigidbodyConstraints.FreezeAll;
            isTraveling = false;
            foreach (var trail in trailRenderers)
            {
                trail.emitting = false;
            }
            meshRenderer.enabled = false;
        } 
        if(hit) return;
        hit = true;

        //Debug.Log("Bullet Hit: " + collision.name);
        
        if(collision.GetComponent<Health>())
        {
            collision.GetComponent<Health>().UpdateHealth(damage, damageType);
            //Debug.Log(collisionInfo.gameObject.name + " took " + damage + " of damage!");
        }
    }

    public void Respawn(Vector3 position)
    {
        isTraveling = true;
        if(!meshRenderer) meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = true;
        rgbd.constraints = RigidbodyConstraints.None;
        rgbd.velocity = Vector3.zero;
        rgbd.angularVelocity = Vector3.zero;
        gravity = moveVector.y;
        speed = moveVector.z;
        transform.position = position;
        transform.SetPositionAndRotation(position, new Quaternion(0,0,0,0));
        rgbd.angularVelocity = Vector3.zero;
        setedVelocity = false;
        StartCoroutine(ResetTrail());
    }
    
    public void DisableBullet(float time)
    {
        StartCoroutine(DisableTimer(time));
    }

    IEnumerator DisableTimer(float time)
    {
        yield return new WaitForSeconds(time);
        setedVelocity = false;

        gameObject.SetActive(false);
    }

    IEnumerator ResetTrail()
    {
        yield return new WaitForEndOfFrame();
        foreach (var trail in trailRenderers)
        {
            trail.emitting = true;
        }
    }

    private void OnValidate() 
    {
        if(bulletType != name) bulletType = name;
    }
}
