using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasHolder;
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
    [SerializeField] private GameObject bloodHolder;
    private Image[] bloodSplaters;
    public Transform BloodHolder => bloodHolder.transform;
    [SerializeField] Animator shieldAnim;
    [SerializeField] GameObject shieldBreak;
    public GameObject eletricDamageVFX;
    public GameObject shieldRecoverVFX;
    public GameObject healthRecoverVFX;
    [SerializeField] private GameObject gameoverAnimUI;
    public GameObject GameOverUI => gameoverAnimUI;

    private void Awake()
    {
        bloodSplaters = bloodHolder.GetComponentsInChildren<Image>();
    }

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
        RebuildGunHolder();
        foreach (var item in gunsSelectables) gunTweens.Add(item.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
        gunTweens.Add(gunsSelectables[index].rectTransform.DOScale(seletedScale, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => RebuildGunHolder()));
    }
    private void RebuildGunHolder() => StartCoroutine(RebuildingLayout(gunsSelectables[0].transform.parent as RectTransform));

    IEnumerator RebuildingLayout(RectTransform rectT)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForSeconds(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        }
    }

    public void UpdateShieldHealthPercentage(float shield, float health)
    {
        shieldAnim.SetFloat("Value", shield);
        if (shieldImage.fillAmount > 0 & shield == 0) InstantiateVFX(shieldBreak);
        shieldImage.fillAmount = shield;
        healthImage.fillAmount = health;
        damageEffect.weight = (ElevateBy(1 - shieldImage.fillAmount, 2)) * 0.7f;

        var dmgPercent = ElevateBy(1 - health, 2);
        for (int i = 0; i < bloodSplaters.Length; i++)
        {
            var indexPercent = ((float)i + 1) / (float)bloodSplaters.Length;
            bloodSplaters[i].gameObject.SetActive(indexPercent <= dmgPercent);
        }
    }

    private float ElevateBy(float value, int elevate)
    {
        if (value == 0) return 0;
        var result = value;
        for (int i = 1; i < elevate; i++)
        {
            result *= value;
        }
        return result;
    }

    public void InstantiateVFX(GameObject vfx)
    {
        Destroy(Instantiate(vfx, canvasHolder.position, transform.rotation, transform), 5f);
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
