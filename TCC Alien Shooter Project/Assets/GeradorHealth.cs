using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GeradorHealth : HealthImmunities
{
    [SerializeField] protected MeshRenderer meshRenderer;
    protected Color curColor = Color.white * -10;

    public override void DestroyCharacter()
    {
        base.DestroyCharacter();
        meshRenderer.material = new Material(meshRenderer.material);

        DOTween.To(() => meshRenderer.material.GetColor("_EmissionColor"), 
            x => curColor = x, Color.white * 10, 4f).SetEase(Ease.InOutSine);
    }

    protected Color EmissionColor() => meshRenderer.material.GetColor("_EmissionColor");

    protected override void Update()
    {
        base.Update();
        if(isDead) meshRenderer.material.SetColor("_EmissionColor", curColor);
    }
}
