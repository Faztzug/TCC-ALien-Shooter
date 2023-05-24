using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using UnityEngine.Rendering.PostProcessing;

public class PlayerShieldHealth : ShieldHealth
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
    //private GameState state;
    public bool dead;
    

    public override void Start()
    {
        base.Start();
        UpdateHealth(maxHealth, DamageType.NULL);
    }

    protected override void Update()
    {
        base.Update();
        if(GameState.GodMode) UpdateHealth(maxHealth, DamageType.NULL);
        UpdateHealthBar();

        if(Input.GetButtonDown("GodMode"))
        {
            GameState.ToogleGodMode();
            Debug.Log("GOD MODE: " + GameState.GodMode);
        }

        // if(damageEffect.weight > 0)
        // {
        //     if(damageTime < 0)
        //     {
        //         damageEffect.weight -= 1f * Time.deltaTime * effectDownMultplier;
        //         damageTime = 0;
        //     }
        //     else
        //     {
        //         damageTime -= 1f * Time.deltaTime;
        //     }
        // }

        if(transform.position.y < fallingDeathHeight) DestroyCharacter();
    }

    private bool updatingHealthThisFrame;
    public override void UpdateHealth(float value, DamageType damageType)
    {
        if(value < 0)
        {
            if (updatingHealthThisFrame) return;
            updatingHealthThisFrame = true;
        }
        
        base.UpdateHealth(value, damageType);
        //bar.fillAmount = health / maxHealth;

        StartCoroutine(EndUpdateHealth());

        var hpPorcentage = Mathf.Abs(health / maxHealth);
        var chgPorcentage = Mathf.Abs(value / maxHealth);


        // if(value < 0)
        // {
        //     damageEffect.weight += chgPorcentage * effectGainMultplier;
        //     damageTime += chgPorcentage * effectTimeMultplier;
        // }
        // else if(value > 0)
        // {
        //     damageEffect.weight -= chgPorcentage * effectGainMultplier;
        //     damageTime -= chgPorcentage * effectTimeMultplier;
        //     if(damageEffect.weight < 0) damageEffect.weight = 0;
        //     if(damageTime < 0) damageTime = 0;
        // }
    }

    public void GainHealth(float value)
    {
        health += Mathf.Clamp(health+value, 0, maxHealth);
    }

    public void UpdateHealthBar() => GameState.mainCanvas.UpdateShieldHealthPercentage(curShield / maxShield, health / maxHealth);

    IEnumerator EndUpdateHealth()
    {
        yield return new WaitForEndOfFrame();
        updatingHealthThisFrame = false;
    }

    public override void DestroyCharacter()
    {
        base.DestroyCharacter();
        if(GameState.IsPlayerDead) return;
        anim.SetBool("Death", true);
        anim.SetTrigger("death");
        
        GameState.IsPlayerDead = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        
        //GameState.mainCanvas.ResumeGame();
        Cursor.lockState = CursorLockMode.None;
        //GameState.mainCanvas.gameOver.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(GameState.mainCanvas.gameOver.GetComponentInChildren<Button>().gameObject);

        GameState.LoadScene(gameObject.scene.name, 5f);
    }
}
