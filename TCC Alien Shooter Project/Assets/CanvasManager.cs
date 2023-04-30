using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Color uiColor;
    [SerializeField] private RectTransform[] gunsSelectables = new RectTransform[]{};
    [SerializeField] private float seletedScale = 1.3f;
    private List<Tween> gunTweens = new List<Tween>();

    void Start()
    {
        
    }

    private void OnValidate() 
    {
        foreach (var image in GetComponentsInChildren<Image>()) image.color = uiColor;
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) text.color = uiColor;
    }

    public void GunSelected(int index)
    {
        foreach (var tween in gunTweens) tween.Kill();
        foreach (var item in gunsSelectables) gunTweens.Add(item.DOScale(1f, 0.2f));
        gunTweens.Add(gunsSelectables[index].DOScale(seletedScale, 0.3f).SetEase(Ease.OutQuad));
    }
}
