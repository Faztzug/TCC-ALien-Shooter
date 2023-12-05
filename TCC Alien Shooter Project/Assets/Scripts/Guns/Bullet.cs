using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Gun parentGun;
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
        var critHit = false;
        if(collisionInfo.collider.gameObject.CompareTag(Gun.kCrtiHitTag))
        {
            Debug.Log("BULEETTT HEADSHOT!" + collisionInfo.collider.gameObject.name);
            damage *= 2;
            critHit = true;
            //GameState.InstantiateSound(headShootSound, collisionInfo.GetContact(0).point);
        }
        if (collisionInfo.collider.TryGetComponent<ModifierDamageArea>(out ModifierDamageArea areaDmgMod))
        {
            var dmgType = damageType;
            DamageModified modifier = areaDmgMod.damageModifiers.Find(d => d.damageType == dmgType | d.damageType == DamageType.AnyDamage);
            if (modifier.damageType == dmgType | modifier.damageType == DamageType.AnyDamage) damage *= modifier.multplier;
        }

        Health health = null;
        if (collisionInfo.rigidbody?.gameObject != null) health = BulletHit(collisionInfo.rigidbody.gameObject);
        else health = BulletHit(collisionInfo.gameObject);
        if (critHit) health?.PlayDamageSound(health.headshootSounds, Health.TimerToUse.critical);
    }
    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.attachedRigidbody?.gameObject != null) BulletHit(other.attachedRigidbody.gameObject, true);
    //     else BulletHit(other.gameObject, true);
    // }
    public virtual Health BulletHit(GameObject collision, bool isTrigger = false)
    {
        if(collision.GetComponentInChildren<Gun>() == parentGun) return null;
        
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
        if(hit) return null;
        hit = true;
        
        var curTransform = collision.transform;
        var healthObj = curTransform.GetComponentInChildren<Health>();
        while (healthObj == null && curTransform.parent != null)
        {
            curTransform = curTransform.parent;
            healthObj = curTransform.GetComponent<Health>();
        }
        healthObj?.UpdateHealth(damage, damageType);
        healthObj?.BleedVFX(transform.position, damageType);
        return healthObj;
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
        ResetTrail();
    }
    
    public void DisableBullet(float time)
    {
        DisableTimer(time);
    }

    async void DisableTimer(float time)
    {
        await Task.Delay((int)(time * 1000));
        if(!this) return;
        setedVelocity = false;

        gameObject.SetActive(false);
    }

    async void ResetTrail()
    {
        await Task.Delay((int)(20));
        if(!this) return;
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
