using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;

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
        foreach (var vfx in GetComponentsInChildren<GunVFXManager>()) 
        {
            foreach (var go in vfx.GetComponentsInChildren<Transform>()) go.gameObject.layer = gameObject.layer;
        }
    }

    void OnDisable()
    {
        StopContinuosFireAudio(primaryFireData);
        StopContinuosFireAudio(secondaryFireData);
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
                    StopContinuosFireAudio(primaryFireData);
                    StopContinuosFireAudio(secondaryFireData);
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
            if(LoadedAmmo >= primaryFireData.ammoCost || primaryFireData.ammoCost == 0)
            {
                if(primaryFireData.continuosFire && Input.GetButton("Fire1") && primaryFireData.fireTimer <= 0) PrimaryFire();
                else if (Input.GetButtonDown("Fire1") && primaryFireData.fireTimer <= 0) PrimaryFire();
            }
            
            if(LoadedAmmo >= secondaryFireData.ammoCost || secondaryFireData.ammoCost == 0)
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
        if(fireStruct.fireSound.IsPlaying) contFireSoundTween = fireStruct.fireSound.audioSource.DOFade(0, 0.2f).SetEase(Ease.InSine)
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

    private void FireFunc(GunFireStruct fireMode)
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

            if(audioSource != null && !audioSource.isPlaying)
            {
                if(audioSource.clip != fireMode.fireSound.clip) fireMode.fireSound.Setup(audioSource);
                audioSource.volume = 0;
                contFireSoundTween.Kill(true);
                contFireSoundTween = audioSource.DOFade(fireMode.fireSound.SFXVolume, 0.2f);
            }
        }
        else
        {
            fireMode.fireSound.PlayOn(audioSource);
        }
    }
    
    public virtual void Shooting(GunFireStruct fireMode, Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : enemyTarget;
        if(!isPlayerGun) transform.LookAt(enemyTarget);

        foreach (var curPoint in gunPointPositions)
        {
            var damageType = fireMode.damageType;
            var laser = curPoint.GetComponentInChildren<GunVFXManager>();
            laser?.SetLaser(curPoint.position, GetRayCastMiddle(curPoint.position, GetRayRange(fireMode), fireMode.piercingRay));

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
                if(fireMode.piercingRay)
                {
                    var allHealths = GetAllHealths(curPoint.position, fireMode);
                    foreach (var health in allHealths)
                    {
                        if(fireMode.continuosFire) health?.UpdateHealth(fireMode.damage * Time.deltaTime, damageType);
                        else health?.UpdateHealth(fireMode.damage, damageType);

                        var bleedPos = health is null ? Vector3.zero : health.transform.position;
                        var hRgbd = health?.GetComponentInChildren<Rigidbody>();
                        if(health != null & hRgbd != null) bleedPos += hRgbd.centerOfMass;
                        health?.BleedVFX(bleedPos, damageType, fireMode.continuosFire);
                    }
                }
                else
                {
                    targetHealth = GetTargetHealth(curPoint.position, fireMode);

                    if(fireMode.continuosFire) targetHealth?.UpdateHealth(fireMode.damage * Time.deltaTime, damageType);
                    else targetHealth?.UpdateHealth(fireMode.damage, damageType);

                    targetHealth?.BleedVFX(GetRayCastMiddle(curPoint.position, GetRayRange(fireMode)), damageType, fireMode.continuosFire);
            
                }
            }
            if(fireMode.notMultpliyGunPoints) break;
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

    public int FullStopLayers => LayerMask.GetMask("Default", "Teto", "ScenaryNavMeshIgnore");
    public Vector3 GetRayCastMiddle(Vector3 gunPoint, float range, bool rayCastAll = false)
    {
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;
        List<RaycastHit> rayHits = new List<RaycastHit>();
        if(rayCastAll) 
        {
            rayHits =  Physics.RaycastAll(gunPoint, aimTransform.forward, range, FullStopLayers, QueryTriggerInteraction.Ignore).ToList();
            if(rayCastAll & rayHits.Count > 0) return rayHits[0].point;
            return aimTransform.position + aimTransform.forward * range;
        }

        if(Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, range, layer, QueryTriggerInteraction.Ignore))
        {
            // if(isPlayerGun)
            // {
            // if(rayHit.collider) Debug.Log(rayHit.collider.gameObject.name?.ToString());
            // if(rayHit.collider) Debug.Log(rayHit.collider.transform.parent?.name?.ToString());
            // if(rayHit.collider & rayHit.collider.transform.parent.parent != null) Debug.Log(rayHit.collider.transform.parent.parent?.name?.ToString());
            // }
            if(rayHit.collider) return rayHit.point;
            else return aimTransform.position + aimTransform.forward * range;
        }
        else
        {
            Debug.DrawLine(aimTransform.position, aimTransform.position + aimTransform.forward * range, Color.green);
            return aimTransform.position + aimTransform.forward * range;
        }
    }

    public Health GetTargetHealth(Vector3 gunPoint,  GunFireStruct fireStruct)
    {
        var range = GetRayRange(fireStruct);
        var diameter = fireStruct.rayDiamanter;
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;
        bool gotHit;
        if(diameter <= 0) gotHit = Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, range, layer, QueryTriggerInteraction.Ignore);
        else gotHit = Physics.SphereCast(gunPoint, diameter, aimTransform.forward, out rayHit, range, layer, QueryTriggerInteraction.Ignore);
        
        if(gotHit)
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

    public List<Health> GetAllHealths(Vector3 gunPoint, GunFireStruct fireStruct)
    {
        var range = GetRayRange(fireStruct);
        var diameter = fireStruct.rayDiamanter;
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit[] rayHits;
        if(diameter <= 0) rayHits = Physics.RaycastAll(gunPoint, aimTransform.forward, range, layer, QueryTriggerInteraction.Ignore);
        else rayHits = Physics.SphereCastAll(gunPoint, diameter, aimTransform.forward, range, layer, QueryTriggerInteraction.Ignore);
        List<Health> healths = new List<Health>();

        if(rayHits.Length > 0)
        {
            foreach (var hit in rayHits)
            {
                if(hit.transform.gameObject.layer == FullStopLayers) break;
                var curTransform = hit.transform;
                var healthObj = curTransform.GetComponentInChildren<Health>();
                while (healthObj == null && curTransform.parent != null)
                {
                    curTransform = curTransform.parent;
                    healthObj = curTransform.GetComponent<Health>();
                }
                if(!healths.Contains(healthObj)) healths.Add(healthObj);
            }
        }
        
        return healths;
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
