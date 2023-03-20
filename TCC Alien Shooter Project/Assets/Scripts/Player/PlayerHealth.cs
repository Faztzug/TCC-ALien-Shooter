using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
//using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : Health
{
    //[SerializeField] private Image bar;
    //private Image bar => GameState.mainCanvas.healthBar;
    //[SerializeField] private PostProcessVolume damageEffect;
    private float damageTime = 0;
    [SerializeField] float effectTimeMultplier = 10;
    [SerializeField] float effectGainMultplier = 2f;
    [SerializeField] float effectDownMultplier = 0.5f;
    [SerializeField] float fallingDeathHeight = -1000;
    public bool IsMaxHealth => health >= maxHealth;
    public AudioSource audioSource;
    public Sound deathSound;
    public Sound[] damageSounds;
    //private GameState state;
    public bool dead;
    private Volume damageEffect;
    

    public override void Start()
    {
        base.Start();
        //bar.fillAmount = health / maxHealth;
        UpdateHealth();
        //damageEffect = GameState.mainCanvas.GetComponentInChildren<Volume>();
        damageEffect.weight = 0f;
        //state = GetComponent<GameState>();
    }

    void Update()
    {
        if(GameState.GodMode) UpdateHealth(maxHealth);

        if(Input.GetButtonDown("GodMode"))
        {
            GameState.ToogleGodMode();
            Debug.Log("GOD MODE: " + GameState.GodMode);
        }

        if(damageEffect.weight > 0)
        {
            if(damageTime < 0)
            {
                damageEffect.weight -= 1f * Time.deltaTime * effectDownMultplier;
                damageTime = 0;
            }
            else
            {
                damageTime -= 1f * Time.deltaTime;
            }
        }

        if(transform.position.y < fallingDeathHeight) DestroyCharacter();
        
    }

    private bool updatingHealthThisFrame;
    public override void UpdateHealth(float value = 0)
    {
        if (updatingHealthThisFrame) return;
        updatingHealthThisFrame = true;
        if(value < -5 && !GameState.IsPlayerDead)
        {
            var index = Random.Range(0, damageSounds.Length);
            damageSounds[index].PlayOn(audioSource);
        }
        base.UpdateHealth(value);
        //bar.fillAmount = health / maxHealth;

        StartCoroutine(EndUpdateHealth());

        var hpPorcentage = Mathf.Abs(health / maxHealth);
        var chgPorcentage = Mathf.Abs(value / maxHealth);

        if(value < 0)
        {
            damageEffect.weight += chgPorcentage * effectGainMultplier;
            damageTime += chgPorcentage * effectTimeMultplier;
        }
        else if(value > 0)
        {
            damageEffect.weight -= chgPorcentage * effectGainMultplier;
            damageTime -= chgPorcentage * effectTimeMultplier;
            if(damageEffect.weight < 0) damageEffect.weight = 0;
            if(damageTime < 0) damageTime = 0;
        }
    }

    IEnumerator EndUpdateHealth()
    {
        yield return new WaitForEndOfFrame();
        updatingHealthThisFrame = false;
    }

    public override void DestroyCharacter()
    {
        if(GameState.IsPlayerDead) return;
        //anim.SetTrigger("Die");
        
        if(audioSource != null) deathSound.PlayOn(audioSource);
        GameState.IsPlayerDead = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        foreach (var item in GetComponentsInChildren<Collider>())
        {
            if(item is CharacterController) continue;
            item.enabled = false;
        }
        foreach (var item in GetComponentsInChildren<MonoBehaviour>())
        {
            if(item == this || item is Movimento || item is GameState) continue;
            item.enabled = false;
        }
        //GameState.mainCanvas.ResumeGame();
        Cursor.lockState = CursorLockMode.None;
        //GameState.mainCanvas.gameOver.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(GameState.mainCanvas.gameOver.GetComponentInChildren<Button>().gameObject);
    }
}
