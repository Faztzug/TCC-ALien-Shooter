using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserVFXManager : MonoBehaviour
{
    [SerializeField] private LineRenderer[] lines;
    [SerializeField] private Transform startVFX;
    [SerializeField] private Transform endVFX;

    public void SetLaser(Vector3 startPoint, Vector3 endPoint)
    {
        SetState(true);
        foreach (var line in lines)
        {
            line.SetPositions(new Vector3[] {startPoint, endPoint});
            Vector3 dir = (endPoint - startPoint).normalized;
            startVFX.position = startPoint;
            endVFX.position = endPoint - (dir / 10);
        }
    }

    public void TurnOffLAser() => SetState(false);

    private void SetState(bool flag)
    {
        foreach (var line in lines)
        {
            line.enabled = flag;
        }
        startVFX.gameObject.SetActive(flag);
        endVFX.gameObject.SetActive(flag);
    }
}
