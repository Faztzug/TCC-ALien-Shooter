using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserVFXManager : GunVFXManager
{
    [SerializeField] private LineRenderer[] lines;

    public override void SetLaser(Vector3 startPoint, Vector3 endPoint)
    {
        SetState(true);
        foreach (var line in lines)
        {
            if(line.useWorldSpace)
            {
                line.SetPosition(0, startPoint);
                line.SetPosition(line.positionCount-1, endPoint);
            }
            else
            {
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(line.positionCount-1, transform.InverseTransformPoint(endPoint));
            }
            //line.SetPositions(new Vector3[] {startPoint, endPoint});
            Vector3 dir = (endPoint - startPoint).normalized;
            startVFX.position = startPoint;
            endVFX.position = endPoint - (dir / 10);
        }
    }

    protected override void SetState(bool flag)
    {
        foreach (var line in lines)
        {
            line.enabled = flag;
        }
        startVFX.gameObject.SetActive(flag);
        endVFX.gameObject.SetActive(flag);
    }
}
