using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] bool continuosDamage = false;
    [SerializeField] bool allPointsGoTarget = true;
    [SerializeField] private int loadedAmmo = 6;
    [SerializeField] private int maxLoadedAmmo = 6;
    [SerializeField] [Range(0, 72)] private int extraAmmo = 12;
    [SerializeField] private int maxExtraAmmo = 72;
    [SerializeField] protected float fire1cooldown = 0.5f;
    [SerializeField] protected float fire2cooldown = 0f;
    protected float fire1timer = 0f;
    protected float fire2timer = 0f;
    [SerializeField] private List<Bullet> bullets;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject Flash;
    [SerializeField] private float shootCooldown = 3f;
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
        foreach (Bullet bullet in bullets)
        {
            bullet.transform.SetParent(null);
            bullet.gameObject.SetActive(false);
        }

        UpdateAmmoText();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected void LateUpdate()
    {
        fire1timer -= Time.deltaTime;
        fire2timer -= Time.deltaTime;
        foreach (var curPoint in gunPointPositions) 
        {
            var line = curPoint.GetComponent<LineRenderer>();
            if(line) line.enabled = false;
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
    virtual protected void PrimaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    virtual protected void SecondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    virtual protected void HoldSencondaryFire()
    {
        fire1timer = fire1cooldown;
        fire2timer = fire2cooldown;
    }
    
    protected void Shooting(Bullet bullet = null)
    {
        var target = movimentoMouse.raycastResult;

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
            var line = curPoint.GetComponent<LineRenderer>();
            if(line) line.enabled = true;
            line?.SetPositions(new Vector3[] {curPoint.position, GetRayCastMiddle(curPoint.position)});

            if(bullet) 
            {
                SpawnBullet(bullet, curPoint.position);
                bullet.transform.LookAt(target);
            }
            else if(continuosDamage)
            {
                if(allPointsGoTarget) movimentoMouse.GetTargetHealth()?.UpdateHealth((float)damage * Time.deltaTime);
                else  GetTargetHealth(curPoint.position)?.UpdateHealth((float)damage * Time.deltaTime);
            }
            else
            {
                if(allPointsGoTarget) movimentoMouse.GetTargetHealth()?.UpdateHealth(damage);
                else  GetTargetHealth(curPoint.position)?.UpdateHealth(damage);
            }
        }
        if(Flash != null)
        {
            var flash = Instantiate(Flash, bullet.transform.position, bullet.transform.rotation);
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
            while (healthObj != null && curTransform.parent != null)
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


    protected void SpawnBullet(Bullet bullet, Vector3 position)
    {
        bullet.hit = false;
        bullet.StopAllCoroutines();
        bullet.Respawn();
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
        if(extraAmmo < maxExtraAmmo)
        {
            extraAmmo += ammount;
            UpdateAmmoText();
            item.DestroyItem();

            if(extraAmmo > maxExtraAmmo) extraAmmo = maxExtraAmmo;
        }
    }
}
