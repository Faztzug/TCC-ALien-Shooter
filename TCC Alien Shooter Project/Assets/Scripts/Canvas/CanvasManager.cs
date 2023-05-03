using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Color uiColor;
    [SerializeField] private List<UIGun> gunsSelectables = new List<UIGun>();
    [SerializeField] private float seletedScale = 1.3f;
    private List<Tween> gunTweens = new List<Tween>();
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image healthImage;
    [SerializeField] private Volume damageEffect;

    void Start()
    {
        
    }

    private void OnValidate() 
    {
        foreach (var uiColorComp in GetComponentsInChildren<UIColor>()) uiColorComp.Color = uiColor;
    }

    public void GunSelected(int index)
    {
        foreach (var tween in gunTweens) tween.Kill();
        foreach (var item in gunsSelectables) gunTweens.Add(item.rectTransform.DOScale(1f, 0.2f));
        gunTweens.Add(gunsSelectables[index].rectTransform.DOScale(seletedScale, 0.3f).SetEase(Ease.OutQuad));
    }

    public void UpdateShieldHealthPercentage(float shield, float health)
    {
        shieldImage.fillAmount = shield;
        healthImage.fillAmount = health;
        damageEffect.weight = 1 - (shieldImage.fillAmount * shieldImage.fillAmount);
    }
    public void UpdateAmmoText(GunType wichGun, float normalizedValue)
    {
        foreach (var item in gunsSelectables.FindAll(g => g.GunType == wichGun))
        {
            item.UpdateAmmo(normalizedValue);
        }
        
    }
}
