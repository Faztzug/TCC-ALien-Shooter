using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct GunFireStruct
{
    public bool notMultpliyGunPoints;
    public DamageType damageType;
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public GameObject Flash;
    public float damage;
    public float maxDistance; //0 equals default distance
    public bool continuosFire;
    public float ammoCost;
    public float fireCooldown;
    [HideInInspector] public float fireTimer;
    public Sound fireSound;
    public bool piercingRay;
    public float rayDiamanter;

    public static bool operator ==(GunFireStruct a, GunFireStruct b)
    {
        return (a.damageType == b.damageType & a.bulletPrefab == b.bulletPrefab & a.damage == b.damage 
        & a.maxDistance == b.maxDistance & a.ammoCost == b.ammoCost & a.fireCooldown == b.fireCooldown);
    }
    public static bool operator !=(GunFireStruct a, GunFireStruct b)
    {
        return !(a.damageType == b.damageType & a.bulletPrefab == b.bulletPrefab & a.damage == b.damage 
        & a.maxDistance == b.maxDistance & a.ammoCost == b.ammoCost & a.fireCooldown == b.fireCooldown);
    }
    public override bool Equals(object obj) => obj is GunFireStruct other && this == other;

    public override int GetHashCode() => base.GetHashCode();
}
