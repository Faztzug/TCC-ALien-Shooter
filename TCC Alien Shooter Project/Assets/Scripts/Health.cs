using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 1f;
    protected float health;
    public float CurHealth => health;
    //[SerializeField] protected AudioSource source;
    //[SerializeField] protected AudioClip damageSound;
    protected Animator anim;
    private EnemyIA thisEnemy;
    public Action onDeath;
    public virtual void Start()
    {
        health = maxHealth;
        thisEnemy = GetComponent<EnemyIA>();
        if(thisEnemy != null) anim = GetComponentInChildren<Animator>();
    }

    public virtual void UpdateHealth(float value, DamageType damageType)
    {
        health += value;

        if(health > maxHealth) health = maxHealth;
        if(health <= 0) DestroyCharacter();

        if(value < 0)
        {
            if(anim != null) anim.SetTrigger("Damage");
            //if(source != null) source.PlayOneShot(damageSound);
            //else Debug.LogError("NO AUDIO SOURCE FOR DAMAGE");
        }
    }

    public virtual void DestroyCharacter()
    {
        onDeath?.Invoke();
        if(thisEnemy != null) thisEnemy.EnemyDeath();
        else if(TryGetComponent<EnemyDrop>(out EnemyDrop drop)) {drop.Drop(); this.gameObject.SetActive(false);}
        else Destroy(this.gameObject);
    }
}
