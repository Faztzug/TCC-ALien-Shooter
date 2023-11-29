using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Animations.Rigging;
//using UnityEngine.Rendering.PostProcessing;

public class PlayerShieldHealth : ShieldHealth
{
    private float defaultMaxShield;
    [SerializeField] float fallingDeathHeight = -1000;
    public bool IsMaxHealth => health >= maxHealth;
    public bool IsMaxShield => curShield >= maxShield;
    public float MaxShield => maxShield;
    //private GameState state;
    public bool dead;
    [SerializeField] private Material playerHealthMat;
    [SerializeField] private Color[] matHealthColors;

    [SerializeField] private Animator gunHolderAnim;
    [SerializeField] private Sound eletricDamage;

    protected override float MinShieldValue => 0;


    public override void Start()
    {
        base.Start();
        
        defaultMaxShield = maxShield;
        var dificulty = GameState.SaveData.gameDificulty;
        if(dificulty == GameDificulty.Easy) maxShield = defaultMaxShield * 1.5f;
        else if(dificulty == GameDificulty.Hard) maxShield = defaultMaxShield / 2;
        curShield = maxShield;
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

        if(transform.position.y < fallingDeathHeight && !GameState.IsPlayerDead) DestroyCharacter();
    }

    private bool updatingHealthThisFrame;
    public override void UpdateHealth(float value, DamageType damageType)
    {
        if(value < 0)
        {
            if (updatingHealthThisFrame) return;
            updatingHealthThisFrame = true;
        }

        if (value < -1 & damageType == DamageType.eletricDamage) Debug.Log("CHOQUE SUPER CHOQUE");
        if (value < -1 & damageType == DamageType.eletricDamage) eletricDamage.PlayOn(audioSource);
        base.UpdateHealth(value, damageType);
        //bar.fillAmount = health / maxHealth;

        StartCoroutine(EndUpdateHealth());

        var hpPorcentage = Mathf.Abs(health / maxHealth);
        var chgPorcentage = Mathf.Abs(value / maxHealth);
    }

    public void GainHealth(float value)
    {
        health += Mathf.Clamp(health+value, 0, maxHealth);
    }

    public void UpdateHealthBar() 
    {
        GameState.mainCanvas.UpdateShieldHealthPercentage(curShield / maxShield, health / maxHealth);
        
        var hpByColor = maxHealth / matHealthColors.Length;
        //Debug.Log("hpByColor " + hpByColor + "colorIndex " + (Mathf.CeilToInt(CurHealth / hpByColor)-1));
        var colorIndex = Mathf.Clamp(Mathf.CeilToInt(CurHealth / hpByColor) -1, 0, matHealthColors.Length-1);
        if(GameState.IsPlayerDead) colorIndex = 0;
        
        var nextColorI = colorIndex == matHealthColors.Length-1 ? colorIndex : colorIndex+1;
        playerHealthMat.color = Color.Lerp(matHealthColors[colorIndex], matHealthColors[nextColorI], CurHealth % hpByColor);
        playerHealthMat.SetColor("_EmissionColor", 
        Color.Lerp(matHealthColors[colorIndex], matHealthColors[nextColorI], CurHealth % hpByColor));
        //DynamicGI.UpdateEnvironment();
    }

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
        gunHolderAnim.SetTrigger("Death");
        
        GameState.IsPlayerDead = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        
        //GameState.mainCanvas.ResumeGame();
        Cursor.lockState = CursorLockMode.None;
        //GameState.mainCanvas.gameOver.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(GameState.mainCanvas.gameOver.GetComponentInChildren<Button>().gameObject);

        GameState.ReloadScene(5f);
    }
}
