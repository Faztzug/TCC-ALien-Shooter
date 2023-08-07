using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public enum GunType
{
    PiranhaGun,
    AcidGun,
    EletricGun,
}

public class Gun : MonoBehaviour
{
    public GunType gunType;
    public GunFireStruct primaryFireData;
    public GunFireStruct secondaryFireData;
    [SerializeField] protected bool isPlayerGun = true;
    [SerializeField] protected bool useTriggerColiderToGetTargets;
    //protected List<Collider> enemysOnTrigger = new List<Collider>();
    public Transform aimTransform;
    [SerializeField] protected float loadedAmmo = 6;
    public float LoadedAmmo 
    {get => loadedAmmo; 
    protected set{loadedAmmo = Mathf.Clamp(value, 0, maxLoadedAmmo); 
    UpdateAmmoText();}}
    [SerializeField] private int maxLoadedAmmo = 6;
    protected List<Bullet> bullets = new List<Bullet>();
    [SerializeField] private float shootCooldown = 30f;
    [SerializeField] protected Transform[] gunPointPositions;
    private Camera cam;
    protected AudioSource audioSource;
    protected Animator anim;
    [SerializeField] protected MovimentoMouse movimentoMouse;
    [HideInInspector] public Vector3 enemyTarget;
    [HideInInspector] public bool enemyHoldingFire;
    private HandGripManager handGripManager;
    protected float ammoRegenPorcent = 0.2f;
    protected float ammoRegenTimer = 1f;
   
    virtual protected void Start()
    {
        cam = Camera.main;
        if(isPlayerGun) aimTransform = cam.transform;

        UpdateAmmoText();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        handGripManager = GetComponentInChildren<HandGripManager>();
    }

    public void AmmoRegen()
    {
        if(LoadedAmmo <= 0 & ammoRegenTimer > 0 
        | (LoadedAmmo <= 0 & (Input.GetButton("Fire1") | Input.GetButton("Fire2")))) 
        {
            ammoRegenTimer -= Time.deltaTime;
        }
        else
        {
            LoadedAmmo += (maxLoadedAmmo * (ammoRegenPorcent / 100)) * Time.deltaTime;
            ammoRegenTimer = 1f;
        }
    }

    protected virtual void LateUpdate()
    {
        primaryFireData.fireTimer -= Time.deltaTime;
        secondaryFireData.fireTimer -= Time.deltaTime;

        if(!isPlayerGun)
        {
            loadedAmmo = 100; //inimigos nunca ficam sem munição
            if(secondaryFireData.fireTimer <= 0 && !enemyHoldingFire)
            {
                //TODO: update to primary fire as well, but will need to keep Lasers serialized
                foreach (var curPoint in gunPointPositions) 
                {
                    var line = curPoint.GetComponentInChildren<GunVFXManager>();
                    if(line) line.TurnOffLAser();
                }
            }
            return;
        }
        
        if(GameState.GodMode) LoadedAmmo = maxLoadedAmmo; 
        //TODO: separate fires modes gun points
        if((primaryFireData.continuosFire && !(Input.GetButton("Fire1") && 
        primaryFireData.fireTimer <= 0)) || loadedAmmo <= 0)
        {
            foreach (var curPoint in gunPointPositions) 
            {
                var line = curPoint.GetComponentInChildren<GunVFXManager>();
                if(line) line.TurnOffLAser();
            }
            StopContinuosFireAudio(primaryFireData);
        }
        else if((secondaryFireData.continuosFire && !(Input.GetButton("Fire2") && 
        secondaryFireData.fireTimer <= 0)) || loadedAmmo <= 0.1f)
        {
            foreach (var curPoint in gunPointPositions) 
            {
                var line = curPoint.GetComponentInChildren<GunVFXManager>();
                if(line) line.TurnOffLAser();
            }
            StopContinuosFireAudio(secondaryFireData);
        }

        if (!GameState.isGamePaused)
        {
            if(LoadedAmmo > 0 || primaryFireData.ammoCost == 0)
            {
                if(primaryFireData.continuosFire && Input.GetButton("Fire1") && primaryFireData.fireTimer <= 0) PrimaryFire();
                else if (Input.GetButtonDown("Fire1") && primaryFireData.fireTimer <= 0) PrimaryFire();
            }
            
            if(LoadedAmmo > 0 || secondaryFireData.ammoCost == 0)
            {
                if(secondaryFireData.continuosFire && Input.GetButton("Fire2") && secondaryFireData.fireTimer <= 0) SecondaryFire();
                else if(Input.GetButtonDown("Fire2") && secondaryFireData.fireTimer <= 0) SecondaryFire();
            }
        }

        foreach (var point in gunPointPositions)
        {
            Debug.DrawRay(point.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.red);
        }
    }

    private Tween contFireSoundTween;
    protected void StopContinuosFireAudio(GunFireStruct fireStruct)
    {
        //TODO: Do fade on begin and on end needs better management to not override each other
        contFireSoundTween.Kill(true);
        if(fireStruct.fireSound.isPlaying) contFireSoundTween = fireStruct.fireSound.audioSource.DOFade(0, 0.2f).SetEase(Ease.InSine)
            .OnComplete(() => {
                if(!(Input.GetButton("Fire2") && fireStruct.fireTimer <= 0) || loadedAmmo <= 0) 
                fireStruct.fireSound.audioSource.Stop();});
    }

    virtual public void PrimaryFire()
    {
        FireFunc(primaryFireData);
        anim?.SetTrigger("Fire1");
    }

    virtual public void SecondaryFire()
    {
        FireFunc(secondaryFireData);
        anim?.SetTrigger("Fire2");
    }

    private void  FireFunc(GunFireStruct fireMode)
    {
        primaryFireData.fireTimer = primaryFireData.fireCooldown;
        secondaryFireData.fireTimer = secondaryFireData.fireCooldown;

        if(!fireMode.continuosFire)  LoadedAmmo -= fireMode.ammoCost;
        else LoadedAmmo -= fireMode.ammoCost * Time.deltaTime;

        if(fireMode.continuosFire)
        {
            if(!audioSource.isPlaying && !audioSource.clip == fireMode.fireSound.clip)
            {
                audioSource.Play();
            }
            else if(!audioSource.isPlaying)
            {
                fireMode.fireSound.PlayOn(audioSource, oneShot: false);
            } 

            if(fireMode.fireSound.audioSource != null && !fireMode.fireSound.audioSource.isPlaying)
            {
                fireMode.fireSound.audioSource.volume = 0;
                contFireSoundTween.Kill(true);
                contFireSoundTween = fireMode.fireSound.audioSource.DOFade(fireMode.fireSound.SFXVolume, 0.2f);
            }
        }
        else
        {
            fireMode.fireSound.PlayOn(audioSource);
        }
    }
    
    public virtual void  Shooting(GunFireStruct fireMode, Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : enemyTarget;
        if(!isPlayerGun) transform.LookAt(enemyTarget);

        foreach (var curPoint in gunPointPositions)
        {
            // if(allPointsGoTarget)  Debug.DrawLine(curPoint.position, target, Color.blue, 10f);
            // else   Debug.DrawLine(curPoint.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.blue, 10f);
            
            var damageType = fireMode.damageType;
            var laser = curPoint.GetComponentInChildren<GunVFXManager>();
            if(laser != null) laser.SetLaser(curPoint.position, GetRayCastMiddle(curPoint.position, GetRayRange(fireMode)));

            Health targetHealth = null;
            if(fireMode.bulletPrefab) 
            {
                var bullet = SpawnBullet(fireMode.bulletPrefab);
                bullet.damageType = damageType;
                bullet.damage = fireMode.damage;
                bullet.transform.LookAt(target);
                ReadyBulletForFire(bullet, curPoint.position);
                bullet.transform.LookAt(target);
            }
            else 
            {
                //if(allPointsGoTarget) targetHealth = movimentoMouse.GetTargetHealth();
                targetHealth = GetTargetHealth(curPoint.position, GetRayRange(fireMode));

                if(fireMode.continuosFire) targetHealth?.UpdateHealth(fireMode.damage * Time.deltaTime, damageType);
                else 
                {
                    targetHealth?.UpdateHealth(fireMode.damage, damageType);
                    Debug.Log("Tracing Damage!");
                }

                targetHealth?.BleedVFX(GetRayCastMiddle(curPoint.position, GetRayRange(fireMode)), damageType, fireMode.continuosFire);
            }
        }
        if(fireMode.Flash != null)
        {
            var flash = Instantiate(fireMode.Flash, bulletPrefab.transform.position, bulletPrefab.transform.rotation);
            Destroy(flash.gameObject, 1f);
        }
    }

    protected float GetRayRange(GunFireStruct fireMode)
    {
        if(fireMode.maxDistance <= 0) return MovimentoMouse.kHorizonPoint;
        else return fireMode.maxDistance;
    }

    public Vector3 GetRayCastMiddle(Vector3 gunPoint, float range)
    {
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;
        
        if(Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, range, layer, QueryTriggerInteraction.Ignore))
        {
            if(rayHit.collider) return rayHit.point;
            else return aimTransform.forward * range;
        }
        else
        {
            Debug.DrawLine(aimTransform.position, aimTransform.position + aimTransform.forward * range, Color.green);
            return aimTransform.position + aimTransform.forward * range;
        }
    }

    public Health GetTargetHealth(Vector3 gunPoint, float range)
    {
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;

        if(Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, range, layer, QueryTriggerInteraction.Ignore))
        {
            var curTransform = rayHit.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            while (healthObj == null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
            }
            return healthObj;
        }
        else
        {
            return null;
        }
    }

    public bool IsACloseObstacleOnFire()
    {
        if(isPlayerGun) return false;
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;
        foreach (var gunPointT in gunPointPositions)
        {
            var gunPoint = gunPointT.position;
            var dir = GameState.PlayerMiddleT.position - gunPoint;
            if(Physics.Raycast(gunPoint, dir.normalized, out rayHit, 10, layer))
            {
                Debug.DrawLine(gunPoint, gunPoint + dir.normalized * 10, Color.yellow, 1f);
                if(rayHit.collider && rayHit.collider.gameObject.layer != LayerMask.NameToLayer("Player")) return true;
            }
        }
        return false;
    }

    protected Bullet SpawnBullet(Bullet bulletPrefab)
    {
        var respawnBullet = bullets.Find(b => b.bulletType == bulletPrefab.bulletType && !b.isTraveling);
        if (respawnBullet != null)
        {
            return respawnBullet;
        }
        else
        {
            var newBullet = Instantiate(bulletPrefab, null);
            newBullet.bulletType = bulletPrefab.name;
            bullets.Add(newBullet);
            return newBullet;
        }
    }
    protected void ReadyBulletForFire(Bullet bullet, Vector3 position)
    {
        bullet.parentGun = this;
        bullet.hit = false;
        bullet.StopAllCoroutines();
        bullet.Respawn(position);
        bullet.transform.position = position;
        bullet.gameObject.SetActive(true);
        bullet.DisableBullet(shootCooldown);
    }

    public void UpdateAmmoText()
    {
        if(!isPlayerGun) return;
        GameState.mainCanvas.UpdateAmmoText(gunType, loadedAmmo / maxLoadedAmmo);
    }

    public void GainAmmo(float ammount, Item item)
    {
        if(LoadedAmmo < maxLoadedAmmo)
        {
            Debug.Log("gaining ammo");
            LoadedAmmo += ammount;
            item?.DestroyItem();

            if(LoadedAmmo > maxLoadedAmmo) LoadedAmmo = maxLoadedAmmo;
        }
    }

    private void OnEnable() 
    {
        handGripManager?.SetGrips();
    }

    private void OnValidate() 
    {
        if(primaryFireData.damage > 0) primaryFireData.damage = -primaryFireData.damage;
        if(secondaryFireData.damage > 0) secondaryFireData.damage = -secondaryFireData.damage;
    }
}
