using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct GunFireStruct
{
    public DamageType damageType;
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public GameObject Flash;
    public float damage;
    public bool continuosFire;
    public float ammoCost;
    public float fireCooldown;
    [HideInInspector] public float fireTimer;
    public Sound fireSound;
}
