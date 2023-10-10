using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GeradorHealth : HealthImmunities
{
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Material offMat;
    [SerializeField] protected Material onMat;
    protected Color curColor = Color.white * -10;

    public override void Start()
    {
        base.Start();
        meshRenderer.material = new Material(offMat);
    }

    public override void DestroyCharacter()
    {
        base.DestroyCharacter();
        var sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => meshRenderer.material.GetColor("_EmissionColor"),
            x => curColor = x, Color.white * -10, 1f).SetEase(Ease.InOutSine));
        sequence.Append(DOTween.To(() => meshRenderer.material.GetColor("_EmissionColor"), 
            x => curColor = x, onMat.GetColor("_EmissionColor"), 3f).SetEase(Ease.InOutSine));
    }

    protected Color EmissionColor() => meshRenderer.material.GetColor("_EmissionColor");

    protected override void Update()
    {
        base.Update();
        if(isDead) meshRenderer.material.SetColor("_EmissionColor", curColor);
    }
}
