using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StatiticLaserVFXManager : LaserVFXManager
{
    [SerializeField] protected float bootingTime = 0.1f;
    [SerializeField] protected float fullIntenseTime = 0.3f;
    [SerializeField] protected float fadingTime = 1f;
    Sequence lineTween;
    Sequence particleTween;
    [HideInInspector] public float multiplierScale = 1f;
    protected override void SetState(bool flag)
    {
        base.SetState(flag);
        if(flag)
        {
            particleTween.Kill();
            particleTween.Append(startVFX.transform.DOScale(1 * multiplierScale,bootingTime)).Insert(0, endVFX.transform.DOScale(1,bootingTime));
            particleTween.AppendInterval(fullIntenseTime);
            particleTween.Append(startVFX.transform.DOScale(0,fadingTime)).Insert(0, endVFX.transform.DOScale(0,fadingTime));

            foreach (var line in lines)
            {
                var defaultWidth = line.widthMultiplier;
                var scaledWidth = defaultWidth * multiplierScale;
                line.widthMultiplier = 0;
                lineTween.Kill();
                lineTween.Append(DOTween.To(w => line.widthMultiplier = w, 0, scaledWidth, bootingTime));
                lineTween.AppendInterval(fullIntenseTime);
                lineTween.Append(DOTween.To(w => line.widthMultiplier = w, scaledWidth, 0, fadingTime));
                lineTween.OnComplete(() => {line.widthMultiplier = defaultWidth; TurnOffLAser();});
            }
            Debug.Log("Super position: " + lines[0].GetPosition(0).ToString() + " / " + lines[0].GetPosition(1).ToString());
        }
    }
}
