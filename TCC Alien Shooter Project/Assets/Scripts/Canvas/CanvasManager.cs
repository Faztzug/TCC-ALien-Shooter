using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private DocumentLoreUIManager PDAmanager;
    private ScrollRect PDAscroll;
    [SerializeField] private RectTransform scrollContent;
    [SerializeField] private RectTransform pdaTextHolder;
    [SerializeField] private Color uiColor;
    [SerializeField] private List<UIGun> gunsSelectables = new List<UIGun>();
    [SerializeField] private float seletedScale = 1.3f;
    private List<Tween> gunTweens = new List<Tween>();
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image healthImage;
    [SerializeField] private Volume damageEffect;

    public bool DoesExitPause()
    {
        if(!pauseMenu.activeSelf && !PDAmanager.gameObject.activeSelf) return false;
        else return !(settingsMenu.activeSelf || tutorial.activeSelf);
    }

    void Start()
    {
        PDAscroll = GetComponentInChildren<ScrollRect>(true);
        foreach (var uiColorComp in GetComponentsInChildren<UIColor>(true)) uiColorComp.Color = uiColor;
        SetPauseMenu(false);
        SetSettingsMenu(false);
        SetTutorial(false);
        SetPDAdocument(false);
    }

    private void OnValidate() 
    {
        foreach (var uiColorComp in GetComponentsInChildren<UIColor>()) uiColorComp.Color = uiColor;
    }

    public void GunSelected(GunType gunType) => GunSelected(gunsSelectables.FindIndex(g => g.GunType == gunType));
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

    public void SetPauseMenu(bool value) 
    {
        pauseMenu.SetActive(value);
    }
    public void SetSettingsMenu(bool value) 
    {
        settingsMenu.SetActive(value);
    }
    public void SetTutorial(bool value) 
    {
        tutorial.SetActive(value);
    }
    public void SetPDAdocument(bool value, LoreDocument ?loreDocument = null) 
    {
        PDAmanager.gameObject.SetActive(value);
        if(loreDocument.HasValue) 
        {
            PDAmanager.SetLoreDucmentUI(loreDocument.Value);
            StartCoroutine(UpdatePDALayoutCourotine());
        }
    }
    IEnumerator UpdatePDALayoutCourotine()
    {
        yield return new WaitForEndOfFrame();
        UpdatePDALayout();
        yield return new WaitForSecondsRealtime(0);
        UpdatePDALayout();
        yield return new WaitForSecondsRealtime(0.1f);
        UpdatePDALayout();
        yield return new WaitForSecondsRealtime(0.5f);
        UpdatePDALayout();
    }
    private void UpdatePDALayout()
    {
        var SCRect = pdaTextHolder;
        Debug.Log(SCRect.name);

        // Debug.Log(SCRect.sizeDelta.ToString());
        // PDAscroll.content.sizeDelta = SCRect.sizeDelta;

        //PDAscroll.content.rect.Set(SCRect.rect.x, SCRect.rect.y, SCRect.rect.width, SCRect.rect.height);
        //Debug.Log(SCRect.rect.ToString());

        scrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SCRect.rect.height);
        Debug.Log(SCRect.rect.height.ToString());


        LayoutRebuilder.ForceRebuildLayoutImmediate(PDAscroll.transform as RectTransform);
    }

    public void ResumeGame() => GameState.PauseGame(false);
    public void QuitGame() => GameState.LoadScene("Menu");
}
