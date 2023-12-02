using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : ShieldHealth
{
    protected override float MinShieldValue => 0;
    [SerializeField] protected RectTransform shieldBar;
    protected RectTransform shieldParent => shieldBar.parent as RectTransform;
    [SerializeField] protected RectTransform healthBar;
    protected RectTransform healthParent => healthBar.parent as RectTransform;
    [SerializeField] Sound victoryJiggle;

    protected override void Update()
    {
        base.Update();
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        var shieldPorcent = curShield / maxShield;
        var healthPorcent = health / maxHealth;
        shieldBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shieldParent.rect.width * shieldPorcent);
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthParent.rect.width * healthPorcent);
    }

    public override void DestroyCharacter()
    {
        base.DestroyCharacter();
        GameState.InstantiateSound(victoryJiggle, this.transform.position);
        
        if (GameState.SaveData.unlockLevelsTo < 3)
        {
            GameState.SaveData.unlockLevelsTo = 3;
            GameState.saveManager.SaveGame(GameState.SaveData);
        }
        GameState.EndLevel();
    }
}
