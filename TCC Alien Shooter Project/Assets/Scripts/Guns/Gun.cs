using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
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
    [SerializeField] private DamageType firstDamageType;
    [SerializeField] private DamageType secondDamageType;
    [SerializeField] bool isPlayerGun = true;
    public Transform aimTransform;
    [SerializeField] bool continuosDamage = false;
    [SerializeField] bool allPointsGoTarget = true;
    [SerializeField] private float loadedAmmo = 6;
    public float LoadedAmmo 
    {get => loadedAmmo; 
    private set{loadedAmmo = Mathf.Clamp(value, 0, maxLoadedAmmo); 
    UpdateAmmoText();}}
    [SerializeField] private int maxLoadedAmmo = 6;
    [SerializeField] private int PrimaryAmmoCost = 1;
    [SerializeField] private int SecondaryAmmoCost = 1;
    [SerializeField] protected float fire1cooldown = 0.5f;
    [SerializeField] protected float fire2cooldown = 0f;
    protected float fire1timer = 0f;
    public float Fire1Timer => fire1timer;
    protected float fire2timer = 0f;
    public float Fire2Timer => fire2timer;
    protected List<Bullet> bullets = new List<Bullet>();
    [SerializeField] protected Bullet bulletPrefab;
    public Bullet GetBulletPrefab => bulletPrefab;
    [SerializeField] private GameObject Flash;
    [SerializeField] private float shootCooldown = 30f;
    [SerializeField] private Transform[] gunPointPositions;
    private Camera cam;
    [SerializeField] protected float damage = -1f;
    protected AudioSource audioSource;
    [SerializeField] protected Sound fire1Sound;
    [SerializeField] protected Sound fire2Sound;
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
        fire1timer -= Time.deltaTime;
        fire2timer -= Time.deltaTime;

        if(!isPlayerGun)
        {
            if(fire2timer <= 0 && !enemyHoldingFire)
            {
                foreach (var curPoint in gunPointPositions) 
                {
                    var line = curPoint.GetComponentInChildren<LaserVFXManager>();
                    if(line) line.TurnOffLAser();
                }
            }
            return;
        }
        
        if(GameState.GodMode) LoadedAmmo = maxLoadedAmmo;
        if(continuosDamage && !(Input.GetButton("Fire2") && fire2timer <= 0) || loadedAmmo <= 0)
        {
            foreach (var curPoint in gunPointPositions) 
            {
                var line = curPoint.GetComponentInChildren<LaserVFXManager>();
                if(line) line.TurnOffLAser();
            }
            if(fire2Sound.isPlaying) fire2Sound.audioSource.DOFade(0, 0.2f).SetEase(Ease.InSine)
            .OnComplete(() => {if(!(Input.GetButton("Fire2") && fire2timer <= 0) || loadedAmmo <= 0) fire2Sound.audioSource.Stop();});
            //if(fire2Sound.isPlaying) fire2Sound.audioSource?.Stop();
        }
        if (!GameState.isGamePaused)
        {
            if (Input.GetButtonDown("Fire1") && fire1timer <= 0  && (LoadedAmmo > 0 || PrimaryAmmoCost == 0)) PrimaryFire();
            else if (Input.GetButtonDown("Fire2") && fire2timer <= 0  && (LoadedAmmo > 0 || SecondaryAmmoCost == 0)) SecondaryFire();

            if(Input.GetButton("Fire2") && fire2timer <= 0  && (LoadedAmmo > 0 || SecondaryAmmoCost == 0)) HoldSencondaryFire();
        }

        foreach (var point in gunPointPositions)
        {
            // var horizonPos = aimTransform.forward * MovimentoMouse.kHorizonPoint;
            // var dir = horizonPos - point.position;
            // var newDirection = Vector3.RotateTowards(point.forward, aimTransform.forward, 360, 0);
            //point.rotation = Quaternion.LookRotation(newDirection);
            //point.LookAt(horizonPos);
            Debug.DrawRay(point.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.red);
            //Debug.DrawRay(aimTransform.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.green);
        }
    }

    virtual public void PrimaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
        if(!continuosDamage) LoadedAmmo -= PrimaryAmmoCost;
        fire1Sound.PlayOn(audioSource);
        anim?.SetTrigger("Fire1");
    }
    virtual public void SecondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
        if(!continuosDamage) LoadedAmmo -= SecondaryAmmoCost;
        fire2Sound.PlayOn(audioSource);
        if(continuosDamage)
        {
            fire2Sound.audioSource.volume = 0;
            fire2Sound.audioSource.DOFade(fire2Sound.SFXVolume, 0.2f);
        }
        anim?.SetTrigger("Fire2");
    }
    virtual public void HoldSencondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
        if(continuosDamage) LoadedAmmo -= SecondaryAmmoCost * Time.deltaTime;
        if(!fire2Sound.isPlaying) fire2Sound.PlayOn(audioSource);
        if(!fire2Sound.isPlaying) Debug.Log("Play Fire2 Sound HOLD");
        anim?.SetTrigger("Fire2");
    }
    
    public void Shooting(DamageType damageType, Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : enemyTarget;

        foreach (var curPoint in gunPointPositions)
        {
            // if(allPointsGoTarget)  Debug.DrawLine(curPoint.position, target, Color.blue, 10f);
            // else   Debug.DrawLine(curPoint.position, aimTransform.forward * MovimentoMouse.kHorizonPoint, Color.blue, 10f);
            
            var laser = curPoint.GetComponentInChildren<LaserVFXManager>();
            if(laser != null) laser.SetLaser(curPoint.position, GetRayCastMiddle(curPoint.position));

            Health targetHealth = null;
            if(bulletPrefab) 
            {
                var bullet = SpawnBullet(bulletPrefab);
                bullet.damageType = damageType;
                bullet.transform.LookAt(target);
                ReadyBulletForFire(bullet, curPoint.position);
                bullet.transform.LookAt(target);
            }
            else 
            {
                if(allPointsGoTarget) targetHealth = movimentoMouse.GetTargetHealth();
                else targetHealth = GetTargetHealth(curPoint.position);

                if(continuosDamage) targetHealth?.UpdateHealth(damage * Time.deltaTime, damageType);
                else targetHealth?.UpdateHealth(damage, damageType);
                targetHealth?.BleedVFX(GetRayCastMiddle(curPoint.position), continuosDamage);
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
        bullet.hit = false;
        bullet.StopAllCoroutines();
        bullet.Respawn(position);
        bullet.transform.position = position;
        bullet.gameObject.SetActive(true);
        bullet.DisableBullet(shootCooldown);
        bullet.damage = damage;
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
}
