using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeColorOnDeathOf : MonoBehaviour
{
    [SerializeField] protected Health healthToObserve;
    [SerializeField] protected MeshRenderer[] meshRenderers;
    [SerializeField] protected Material offMat;
    [SerializeField] protected Material onMat;
    protected Color curColor = Color.white * -10;

    public void Start()
    {
        foreach (var mesh in meshRenderers)
        {
            mesh.material = new Material(mesh.material);
            mesh.material.SetColor("_EmissionColor", offMat.GetColor("_EmissionColor"));
        }
        
        healthToObserve.onDeath += DeathChange;
    }

    public void DeathChange()
    {
        foreach (var mesh in meshRenderers)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => mesh.material.GetColor("_EmissionColor"),
                x => curColor = x, Color.white * -10, 1f).SetEase(Ease.InOutSine));
            sequence.Append(DOTween.To(() => mesh.material.GetColor("_EmissionColor"),
                x => curColor = x, onMat.GetColor("_EmissionColor"), 3f).SetEase(Ease.InOutSine));
        }
        
    }

    protected void Update()
    {
        if(healthToObserve.isDead) foreach (var mesh in meshRenderers) mesh.material.SetColor("_EmissionColor", curColor);
    }

    private void OnDestroy()
    {
        healthToObserve.onDeath -= DeathChange;
    }
}
