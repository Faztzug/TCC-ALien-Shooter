using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;
using UnityEngine.Animations.Rigging;

[Serializable]
public struct DamageModified
{
    public DamageType damageType;
    [Range(0,4)] public float multplier;

    public override string ToString()
    {
        return damageType.ToString() + " x" + multplier;
    }

    public DamageModified(DamageType damageType, float multplier)
    {
        this.damageType = damageType;
        this.multplier = multplier;
    }

    public static bool operator ==(DamageModified a, DamageModified b)
    {
        return (a.damageType == b.damageType);
    }
    public static bool operator !=(DamageModified a, DamageModified b)
    {
        return (a.damageType != b.damageType);
    }
    public override bool Equals(object obj) => obj is DamageModified other && this == other;
    public override int GetHashCode() => base.GetHashCode();
}

public class Health : MonoBehaviour
{
    public float maxHealth = 1f;
    protected float health;
    public float CurHealth => health;
    protected Animator anim;
    protected EnemyIA thisEnemy;
    public Action onDeath;
    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private GameObject DeathVFX;
    public Sound[] damageSounds;
    public Sound deathSound;
    public AudioSource audioSource;
    protected float damageSoundTimer;
    public List<DamageModified> damageModifiers = new List<DamageModified>();

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
        if(damageModifiers.Count >= 1)
        {
            DamageModified modifier = damageModifiers.Find(d => d.damageType == damageType | d.damageType == DamageType.AnyDamage);
            if(modifier.damageType == damageType | modifier.damageType == DamageType.AnyDamage) 
            Debug.Log("modified damage of " + modifier.damageType + value+"*"+modifier.multplier
            + " to: " + (value *= modifier.multplier));
            if(modifier.damageType == damageType | modifier.damageType == DamageType.AnyDamage) value *= modifier.multplier;
        }
        
        health += value;

        if(health > maxHealth) health = maxHealth;
        if(health <= 0) DestroyCharacter();

        if(value < 0)
        {
            if(anim != null) anim.SetTrigger("Damage");
            if(thisEnemy != null) thisEnemy.OnDamage(damageType);
            //if(source != null) source.PlayOneShot(damageSound);
            //else Debug.LogError("NO AUDIO SOURCE FOR DAMAGE");
        }

        if(value < 0 && health >= 0)
        {
            if(damageSounds != null & damageSounds.Length > 0 & damageSoundTimer < 0 && damageSounds.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, damageSounds.Length);
                damageSounds[index].PlayOn(audioSource);
                damageSoundTimer = 1f;
                //Debug.Log("Health dmaage Sound " + name);
            }
        }
    }

    protected float bloodVfxTimer = 0f;
    public virtual void BleedVFX(Vector3 position, DamageType damageType, bool isContinuos = false)
    {
        if(isContinuos & bloodVfxTimer > 0) return;
        //if(bloodVFX == null) Debug.Log(name + " Não possui vfx de sangue!");
        else if(bloodVFX != null) GameObject.Destroy(GameObject.Instantiate(bloodVFX, position, Quaternion.identity, null), 3f);
        if(isContinuos) bloodVfxTimer = 0.5f;
    }

    public virtual void DestroyCharacter()
    {
        onDeath?.Invoke();

        if(DeathVFX != null) GameObject.Destroy(GameObject.Instantiate(DeathVFX, transform.position, transform.rotation, null), 5f);
        if(audioSource == null | anim == null) GameState.InstantiateSound(deathSound, transform.position);
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
                || script is CanvasManager || script is Volume || LayerMask.LayerToName(script.gameObject.layer) == "UI"
                || script is RigBuilder && this is PlayerShieldHealth) 
                {
                    continue;
                }

                if(script is MovimentoMouse) script.enabled = false;
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
