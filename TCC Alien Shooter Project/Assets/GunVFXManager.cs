using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunVFXManager : MonoBehaviour
{
    [SerializeField] protected Transform startVFX;
    [SerializeField] protected Transform endVFX;

    public abstract void SetLaser(Vector3 startPoint, Vector3 endPoint);

    public void TurnOffLAser() => SetState(false);

    protected abstract void SetState(bool flag);
}
