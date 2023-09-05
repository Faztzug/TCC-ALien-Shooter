using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricVFXManager : GunVFXManager
{
    [SerializeField] protected Transform[] points;
    public override void SetLaser(Vector3 startPoint, Vector3 endPoint)
    {
        SetState(true);
        startVFX.position = startPoint;
        endVFX.position = endPoint;

        for (int i = 0; i < points.Length; i++)
        {
            if(i == 0 | i == points.Length - 1) continue;

            var point = points[i];

            var straightLocalPos = Vector3.Lerp(point.InverseTransformPoint(startPoint), 
            point.InverseTransformPoint(endPoint), i / points.Length);

            var newLocalPos = straightLocalPos;
            newLocalPos.y = point.localPosition.y;
            point.localPosition = newLocalPos;
        }
    }

    protected override void SetState(bool flag)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(flag);
        }
    }
}
