using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{
    public float maxHealth = 1f;
    protected float health;
    public float CurHealth => health;
    //[SerializeField] protected AudioSource source;
    //[SerializeField] protected AudioClip damageSound;
    protected Animator anim;
    protected EnemyIA thisEnemy;
    public Action onDeath;
    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private GameObject DeathVFX;
    [SerializeField] protected Transform handTransform;
    public Sound[] damageSounds;
    public Sound deathSound;
    public AudioSource audioSource;
    protected float damageSoundTimer;

    public virtual void Start()
    {
        health = maxHealth;
        thisEnemy = GetComponent<EnemyIA>();
        anim = GetComponentInChildren<Animator>();
        var getAudio = GetComponentInChildren<AudioSource>();
        if(getAudio != null) audioSource = getAudio;
    }

    protected virtual void Update() 
    {
        bloodVfxTimer -= Time.deltaTime;
        damageSoundTimer -= Time.deltaTime;
    }

    public virtual void UpdateHealth(float value, DamageType damageType)
    {
        health += value;

        if(health > maxHealth) health = maxHealth;
        if(health <= 0) DestroyCharacter();

        if(value < 0)
        {
            if(anim != null) anim.SetTrigger("Damage");
            if(thisEnemy != null) thisEnemy.OnDamage();
            //if(source != null) source.PlayOneShot(damageSound);
            //else Debug.LogError("NO AUDIO SOURCE FOR DAMAGE");
        }

        if(value < 0 && health >= 0)
        {
            if(damageSoundTimer < 0 && damageSounds.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, damageSounds.Length);
                damageSounds[index]?.PlayOn(audioSource);
                damageSoundTimer = 1f;
                Debug.Log("Health dmaage Sound " + name);
            }
        }
    }

    protected float bloodVfxTimer = 0f;
    public virtual void BleedVFX(Vector3 position, DamageType damageType, bool isContinuos = false)
    {
        if(isContinuos & bloodVfxTimer > 0) return;
        if(bloodVFX == null) Debug.Log(name + " Não possui vfx de sangue!");
        else GameObject.Destroy(GameObject.Instantiate(bloodVFX, position, Quaternion.identity, null), 3f);
        if(isContinuos) bloodVfxTimer = 0.5f;
    }

    public virtual void DestroyCharacter()
    {
        onDeath?.Invoke();

        if(DeathVFX != null) GameObject.Destroy(GameObject.Instantiate(DeathVFX, transform.position, transform.rotation, null), 5f);
        if(audioSource != null) GameState.InstantiateSound(deathSound, transform.position);
        else deathSound.PlayOn(audioSource);

        if(anim != null)
        {
            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                if(collider is CharacterController) continue;
                collider.enabled = false;
            }
            foreach (var script in GetComponentsInChildren<MonoBehaviour>())
            {
                if(script == this || script is Movimento || script is GameState 
                || script is CanvasManager || script is Volume || LayerMask.LayerToName(script.gameObject.layer) == "UI") 
                {
                    continue;
                }

                if(script is MovimentoMouse) script.enabled = false;
                if(script is Gun && handTransform != null) script.transform.SetParent(handTransform);
                if(script is LaserVFXManager) (script as LaserVFXManager).TurnOffLAser();
                script.enabled = false;
            }
        }
        
        if(thisEnemy != null) 
        {
            thisEnemy.EnemyDeath();
        }
        else if(TryGetComponent<EnemyDrop>(out EnemyDrop drop)) 
        {
            drop.Drop(); 
            this.gameObject.SetActive(false);
        }
        else if(anim == null || anim.parameterCount == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
