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
    public Sound[] extraDamageSounds;
    public Sound[] headshootSounds;
    public Sound deathSound;
    public AudioSource audioSource;
    protected float bloodVfxTimer = 0f;
    protected float damageSoundTimer;
    public bool CanDoDamageSound => damageSoundTimer < 0;
    protected float alternateDamageSoundTimer;
    public List<DamageModified> damageModifiers = new List<DamageModified>();
    [SerializeField] protected bool doesDestroyOnDeath = true;
    public UnityEvent OnDeath;
    public const float kEasyHealthPctg = 0.8f;

    public virtual void Start()
    {
        thisEnemy = GetComponent<EnemyIA>();
        if (GameState.SaveData.gameDificulty == GameDificulty.Easy & !(this is PlayerShieldHealth) & thisEnemy) maxHealth *= kEasyHealthPctg;
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        if(audioSource == null) GetComponentInChildren<AudioSource>();
    }

    protected virtual void Update() 
    {
        bloodVfxTimer -= Time.deltaTime;
        damageSoundTimer -= Time.deltaTime;
        alternateDamageSoundTimer -= Time.deltaTime;
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
        }

        if(value < 0 && health >= 0)
        {
            PlayDamageSound(damageSounds);
            PlayDamageSound(extraDamageSounds, TimerToUse.alternate);
        }
    }

    public enum TimerToUse
    {
        normal,
        alternate,
        critical,
    }

    public void PlayDamageSound(Sound[] sounds, TimerToUse timer = TimerToUse.normal)
    {
        var normal = timer == TimerToUse.normal;
        var aternateTimer = timer == TimerToUse.alternate;
        var crit = timer == TimerToUse.critical;

        if (sounds != null & sounds.Length > 0 & 
            (crit || (damageSoundTimer < 0f) || (aternateTimer & alternateDamageSoundTimer < 0f)))
        {
            var index = UnityEngine.Random.Range(0, sounds.Length);
            GameState.InstantiateSound(sounds[index], this.transform.position);

            if (aternateTimer) alternateDamageSoundTimer = 0.2f;
            else if(normal) damageSoundTimer = 0.2f;

            if (!(this is PlayerShieldHealth)) Debug.Log("Playeing Damage: " + sounds[index].clip.name);
        }
    }

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
