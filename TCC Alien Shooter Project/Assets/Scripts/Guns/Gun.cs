using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] bool continuosDamage = false;
    [SerializeField] private int loadedAmmo = 6;
    [SerializeField] private int maxLoadedAmmo = 6;
    [SerializeField] [Range(0, 72)] private int extraAmmo = 12;
    [SerializeField] private int maxExtraAmmo = 72;
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

    void Update()
    {
        if (!GameState.isGamePaused)
        {
            if (Input.GetButtonDown("Fire1")) PrimaryFire();
            else if (Input.GetButtonDown("Fire2")) SecondaryFire();
            else if (Input.GetButtonDown("Reload")) Reload();

            if(Input.GetButton("Fire2")) HoldSencondaryFire();
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
        
    }
    virtual protected void SecondaryFire()
    {
        
    }
    virtual protected void HoldSencondaryFire()
    {
        
    }
    
    protected void Shooting(Bullet bullet = null)
    {
        var target = movimentoMouse.raycastResult;
        if(target != Vector3.zero)
        {
            foreach (var point in gunPointPositions)
            {
                Debug.DrawLine(point.position, target, Color.blue, 10f);
                if(bullet) 
                {
                    SpawnBullet(bullet, point.position);
                    bullet.transform.LookAt(target);
                }
                else if(continuosDamage)
                {
                    movimentoMouse.GetTargetHealth()?.UpdateHealth((float)damage * Time.deltaTime);
                }
                else movimentoMouse.GetTargetHealth()?.UpdateHealth(damage);
            }
        }
        if(Flash != null)
        {
            var flash = Instantiate(Flash, bullet.transform.position, bullet.transform.rotation);
            Destroy(flash.gameObject, 1f);
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
