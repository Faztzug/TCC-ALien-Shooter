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
    [SerializeField] private Material playerHealthMat;
    [SerializeField] private Color[] matHealthColors;

    [SerializeField] private Animator gunHolderAnim;

    protected override float MinShieldValue => 0;


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

        GameState.LoadScene(gameObject.scene.name, 5f);
    }
}
