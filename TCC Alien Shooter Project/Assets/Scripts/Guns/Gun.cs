using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] bool isPlayerGun = true;
    [SerializeField] bool continuosDamage = false;
    [SerializeField] bool allPointsGoTarget = true;
    [SerializeField] private float loadedAmmo = 6;
    public float LoadedAmmo => loadedAmmo;
    [SerializeField] private int maxLoadedAmmo = 6;
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
    //[SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] protected float damage = -1f;
    protected AudioSource audioSource;
    protected Sound fireSound;
    protected Sound reloadSound;
    protected Sound reloadFailSound;
    protected Animator anim;
    [SerializeField] protected MovimentoMouse movimentoMouse;
   
    virtual protected void Start()
    {
        cam = Camera.main;

        UpdateAmmoText();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected void LateUpdate()
    {
        fire1timer -= Time.deltaTime;
        fire2timer -= Time.deltaTime;

        if(!isPlayerGun) return;

        if(!(Input.GetButton("Fire2") && fire2timer <= 0))
        {
            foreach (var curPoint in gunPointPositions) 
            {
                var line = curPoint.GetComponentInChildren<LaserVFXManager>();
                if(line) line.TurnOffLAser();
            }
        }
        if (!GameState.isGamePaused)
        {
            if (Input.GetButtonDown("Fire1") && fire1timer <= 0) PrimaryFire();
            else if (Input.GetButtonDown("Fire2") && fire2timer <= 0) SecondaryFire();
            else if (Input.GetButtonDown("Reload")) Reload();

            if(Input.GetButton("Fire2") && fire2timer <= 0) HoldSencondaryFire();
        }

        foreach (var point in gunPointPositions)
        {
            Debug.DrawRay(point.position, cam.transform.forward * 500f, Color.red);
        }
    }

    virtual protected void Reload()
    {
        
    }
    virtual public void PrimaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    virtual public void SecondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    virtual public void HoldSencondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    
    public void Shooting(Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : GameState.PlayerTransform.position;

        foreach (var curPoint in gunPointPositions)
        {
            if(allPointsGoTarget) 
            {
                Debug.DrawLine(curPoint.position, target, Color.blue, 10f);
            }
            else 
            {
                Debug.DrawLine(curPoint.position, cam.transform.forward * 500, Color.blue, 10f);
            }
            var laser = curPoint.GetComponentInChildren<LaserVFXManager>();
            if(laser) laser.SetLaser(curPoint.position, GetRayCastMiddle(curPoint.position));

            if(bulletPrefab) 
            {
                var bullet = SpawnBullet(bulletPrefab);
                bullet.transform.LookAt(target);
                ReadyBulletForFire(bullet, curPoint.position);
                bullet.transform.LookAt(target);
                loadedAmmo -= 1;
            }
            else if(continuosDamage)
            {
                if(allPointsGoTarget) movimentoMouse.GetTargetHealth()?.UpdateHealth((float)damage * Time.deltaTime);
                else  GetTargetHealth(curPoint.position)?.UpdateHealth((float)damage * Time.deltaTime);
                loadedAmmo -= Time.deltaTime;
            }
            else
            {
                if(allPointsGoTarget) movimentoMouse.GetTargetHealth()?.UpdateHealth(damage);
                else  GetTargetHealth(curPoint.position)?.UpdateHealth(damage);
                loadedAmmo -= 1;
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
        var layer = movimentoMouse.GetLayers();
        RaycastHit rayHit;

        if(Physics.Raycast(gunPoint, cam.transform.forward, out rayHit, 500, layer))
        {
            if(!rayHit.collider)Debug.Log("no collision");
            if(rayHit.collider) return rayHit.point;
            else return cam.transform.forward * 500;
        }
        else
        {
            Debug.Log("no hit");
            return cam.transform.forward * 500;
        }
    }

    public Health GetTargetHealth(Vector3 gunPoint)
    {
        var layer = movimentoMouse.GetLayers();
        RaycastHit rayHit;

        if(Physics.Raycast(gunPoint, cam.transform.forward, out rayHit, 500, layer))
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
        //bullet.transform.localRotation = cam.transform.rotation;
        bullet.gameObject.SetActive(true);
        bullet.DisableBullet(shootCooldown);
        bullet.damage = damage;
    }

    public void UpdateAmmoText()
    {
        //ammoText.text = loadedAmmo + " / " + extraAmmo;
        //ammoText.text = extraAmmo.ToString();
    }

    public void GainAmmo(int ammount, Item item)
    {
        if(loadedAmmo < maxLoadedAmmo)
        {
            loadedAmmo += ammount;
            UpdateAmmoText();
            item.DestroyItem();

            if(loadedAmmo > maxLoadedAmmo) loadedAmmo = maxLoadedAmmo;
        }
    }
}
