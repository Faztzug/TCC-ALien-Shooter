using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] bool isPlayerGun = true;
    public Transform aimTransform;
    [SerializeField] bool allPointsGoTarget = true;
    [SerializeField] private float loadedAmmo = 6;
    public float LoadedAmmo 
    {get => loadedAmmo; 
    private set{loadedAmmo = Mathf.Clamp(value, 0, maxLoadedAmmo); 
    UpdateAmmoText();}}
    [SerializeField] private int maxLoadedAmmo = 6;
    protected List<Bullet> bullets = new List<Bullet>();
    [SerializeField] private GameObject Flash;
    [SerializeField] private float shootCooldown = 30f;
    [SerializeField] private Transform[] gunPointPositions;
    private Camera cam;
    protected AudioSource audioSource;
    protected Animator anim;
    [SerializeField] protected MovimentoMouse movimentoMouse;
    [HideInInspector] public Vector3 enemyTarget;
    [HideInInspector] public bool enemyHoldingFire;
    private HandGripManager handGripManager;
   
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

    protected void LateUpdate()
    {
        primaryFireData.fireTimer -= Time.deltaTime;
        secondaryFireData.fireTimer -= Time.deltaTime;

        if(!isPlayerGun)
        {
            loadedAmmo = 100; //inimigos nunca ficam sem munição
            if(secondaryFireData.fireTimer <= 0 && !enemyHoldingFire)
            {
                //TODO: update to primary fire as well
                foreach (var curPoint in gunPointPositions) 
                {
                    var line = curPoint.GetComponentInChildren<LaserVFXManager>();
                    if(line) line.TurnOffLAser();
                }
            }
            return;
        }
        
        if(GameState.GodMode) LoadedAmmo = maxLoadedAmmo;
        if(secondaryFireData.continuosFire && !(Input.GetButton("Fire2") && secondaryFireData.fireTimer <= 0) || loadedAmmo <= 0)
        {
            //TODO: need fix to carry separate laser for modes of fire
            foreach (var curPoint in gunPointPositions) 
            {
                var line = curPoint.GetComponentInChildren<LaserVFXManager>();
                if(line) line.TurnOffLAser();
            }
            //TODO: Do fade on begin and on end needs better management to not override each other
            if(secondaryFireData.fireSound.isPlaying) secondaryFireData.fireSound.audioSource.DOFade(0, 0.2f).SetEase(Ease.InSine)
            .OnComplete(() => {
                if(!(Input.GetButton("Fire2") && secondaryFireData.fireTimer <= 0) || loadedAmmo <= 0) 
                secondaryFireData.fireSound.audioSource.Stop();});
        }
        if (!GameState.isGamePaused)
        {
            if(LoadedAmmo > 0 || primaryFireData.ammoCost == 0)
            {
                if (Input.GetButtonDown("Fire1") && primaryFireData.fireTimer <= 0) PrimaryFire();
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

    virtual public void PrimaryFire()
    {
        //TODO: update to have hold mode like secondary fire
        primaryFireData.fireTimer = primaryFireData.fireCooldown;
        secondaryFireData.fireTimer = secondaryFireData.fireCooldown;
        if(!primaryFireData.continuosFire) LoadedAmmo -= primaryFireData.ammoCost;
        primaryFireData.fireSound.PlayOn(audioSource);
        anim?.SetTrigger("Fire1");
    }

    virtual public void SecondaryFire()
    {
        primaryFireData.fireTimer = primaryFireData.fireCooldown;
        secondaryFireData.fireTimer = secondaryFireData.fireCooldown;
        if(!secondaryFireData.continuosFire) 
        {
            LoadedAmmo -= secondaryFireData.ammoCost;
            LoadedAmmo -= secondaryFireData.ammoCost * Time.deltaTime;
        }

        if(secondaryFireData.continuosFire)
        {
            if(!audioSource.isPlaying && !audioSource.clip == secondaryFireData.fireSound.clip)
            {
                audioSource.Play();
            }
            else if(!audioSource.isPlaying)
            {
                secondaryFireData.fireSound.PlayOn(audioSource, oneShot: false);
            } 

            if(secondaryFireData.fireSound.audioSource != null && !secondaryFireData.fireSound.audioSource.isPlaying)
            {
                secondaryFireData.fireSound.audioSource.volume = 0;
                secondaryFireData.fireSound.audioSource.DOFade(secondaryFireData.fireSound.SFXVolume, 0.2f);
            }
        }
        else
        {
            secondaryFireData.fireSound.PlayOn(audioSource);
        }
        anim?.SetTrigger("Fire2");
    }
    // virtual public void HoldSencondaryFire()
    // {
    //     fire1timer = fire1cooldown;
    //     fire2timer = fire2cooldown;
    //     if(secondaryFireData.continuosFire) LoadedAmmo -= SecondaryAmmoCost * Time.deltaTime;
    //     if(!secondaryFireData.fireSound.isPlaying) secondaryFireData.fireSound.PlayOn(audioSource);
    //     if(!secondaryFireData.fireSound.isPlaying) Debug.Log("Play Fire2 Sound HOLD");
    //     anim?.SetTrigger("Fire2");
    // }
    
    public void Shooting(GunFireStruct fireMode, Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : enemyTarget;
        if(!isPlayerGun) transform.LookAt(enemyTarget);

        foreach (var curPoint in gunPointPositions)
        {
            // if(allPointsGoTarget)  Debug.DrawLine(curPoint.position, target, Color.blue, 10f);
            // else   Debug.DrawLine(curPoint.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.blue, 10f);
            
            var damageType = fireMode.damageType;
            var laser = curPoint.GetComponentInChildren<LaserVFXManager>();
            if(laser != null) laser.SetLaser(curPoint.position, GetRayCastMiddle(curPoint.position));

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
                if(allPointsGoTarget) targetHealth = movimentoMouse.GetTargetHealth();
                else targetHealth = GetTargetHealth(curPoint.position);

                if(fireMode.continuosFire) targetHealth?.UpdateHealth(fireMode.damage * Time.deltaTime, damageType);
                else targetHealth?.UpdateHealth(fireMode.damage, damageType);
                targetHealth?.BleedVFX(GetRayCastMiddle(curPoint.position), damageType, fireMode.continuosFire);
            }
            
        }
        if(Flash != null)
        {
            var flash = Instantiate(Flash, bulletPrefab.transform.position, bulletPrefab.transform.rotation);
            Destroy(flash.gameObject, 1f);
        }
    }
    public Vector3 GetRayCastMiddle(Vector3 gunPoint)
    {
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;
        
        if(Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, MovimentoMouse.kHorizonPoint, layer))
        {
            if(rayHit.collider) return rayHit.point;
            else return aimTransform.forward * MovimentoMouse.kHorizonPoint;
        }
        else
        {
            Debug.DrawLine(aimTransform.position, aimTransform.position + aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.green);
            return aimTransform.position + aimTransform.forward * MovimentoMouse.kHorizonPoint;
        }
    }

    public Health GetTargetHealth(Vector3 gunPoint)
    {
        var layer = MovimentoMouse.GetLayers(isPlayerGun);
        RaycastHit rayHit;

        if(Physics.Raycast(gunPoint, aimTransform.forward, out rayHit, MovimentoMouse.kHorizonPoint, layer))
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
