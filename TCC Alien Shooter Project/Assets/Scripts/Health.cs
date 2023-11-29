using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

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
    private float _health;
    protected float health { get => _health; set { _health = Mathf.Clamp(value, 0f, maxHealth); } }
    public float CurHealth => health;
    [HideInInspector] public bool isDead = false;
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
    [SerializeField] protected bool doesDestroyOnDeath = true;
    public UnityEvent OnDeath;

    public virtual void Start()
    {
        health = maxHealth;
        thisEnemy = GetComponent<EnemyIA>();
        anim = GetComponentInChildren<Animator>();
        if(audioSource == null) GetComponentInChildren<AudioSource>();
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
            if(modifier.damageType == damageType | modifier.damageType == DamageType.AnyDamage) value *= modifier.multplier;
        }
        
        health += value;
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
            PlayDamageSound(damageSounds);
        }
    }

    protected void PlayDamageSound(Sound[] sounds)
    {
        if (sounds != null & sounds.Length > 0 & damageSoundTimer < 0)
        {
            var index = UnityEngine.Random.Range(0, sounds.Length);
            GameState.InstantiateSound(sounds[index], this.transform.position);
            //sounds[index].PlayOn(audioSource);
            damageSoundTimer = 0.2f;
            //Debug.Log("Health dmaage Sound " + name);
        }
    }

    protected float bloodVfxTimer = 0f;
    public virtual void BleedVFX(Vector3 position, DamageType damageType, bool isContinuos = false)
    {
        if(isContinuos & bloodVfxTimer > 0) return;
        if(bloodVFX != null) 
        {
            var n = isContinuos ? 1 : 3;
            for (int i = 0; i < n; i++)
            {
                var go = GameObject.Instantiate(bloodVFX, position, Quaternion.identity, null);
                if(this.gameObject.GetComponentInChildren<Animator>() != null) go.transform.parent = this.transform;
                GameObject.Destroy(go, 3f);
            }
        }
        if(isContinuos) bloodVfxTimer = 0.2f;
    }

    public virtual void DestroyCharacter()
    {
        if (isDead) return;

        if (thisEnemy != null)
        {
            thisEnemy.EnemyDeath();
        }
        else if (TryGetComponent<EnemyDrop>(out EnemyDrop drop))
        {
            drop.Drop();
            this.gameObject.SetActive(false);
        }
        else if ((anim == null || anim.parameterCount == 0) & doesDestroyOnDeath)
        {
            Destroy(this.gameObject, 0f);
        }

        isDead = true;

        onDeath?.Invoke();
        OnDeath?.Invoke();

        GetComponentInChildren<AudioSource>()?.Stop();
        if(DeathVFX != null) GameObject.Destroy(GameObject.Instantiate(DeathVFX, transform.position, transform.rotation, null), 5f);
        if(audioSource == null || anim == null) GameState.InstantiateSound(deathSound, transform.position);
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
                if(this is PlayerShieldHealth && script == this || script is Movimento || script is GameState 
                || script is CanvasManager || script is Volume || LayerMask.LayerToName(script.gameObject.layer) == "UI") 
                {
                    continue;
                }

                if(script is Gun) (script as Gun).TurnOffLasers();
                if(script is EnemyIA) continue;
                if(script is MovimentoMouse) script.enabled = false;
                if(script is LaserVFXManager) (script as LaserVFXManager).TurnOffLAser();
                script.enabled = false;
            }
        }
    }
}
