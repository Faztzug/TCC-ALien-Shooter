using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EletricGun : Gun
{
    private float chargingPower;
    public float ChargingPower {get => chargingPower; set => chargingPower = Mathf.Clamp(value, 0, maxChargePower);}
    [SerializeField] private float minChargePower = 0.5f;
    [SerializeField] private float maxChargePower = 2f;
    private float baseSnipeDamage;
    private float baseSnipeCost;
    [SerializeField] private GameObject superLaserVFX;
    [SerializeField] private GameObject chargingVFX;

    protected override void Start()
    {
        base.Start();
        chargingVFX.SetActive(false);
        baseSnipeDamage = secondaryFireData.damage;
        baseSnipeCost = secondaryFireData.ammoCost;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if(Input.GetButton("Fire1") | Input.GetButtonUp("Fire2") & ChargingPower < minChargePower) SetChargeOff();
        if(!(LoadedAmmo >= secondaryFireData.ammoCost &  secondaryFireData.fireTimer <= 0)) return;

        if(Input.GetButton("Fire2")) Charging();
        else if(Input.GetButtonUp("Fire2")) SecondaryFire();
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();
        Shooting(primaryFireData);
    }

    protected void Charging()
    {
        ChargingPower += Time.deltaTime;
        chargingVFX.SetActive(true);
        chargingVFX.transform.localScale = Vector3.one * ChargingPower;
    }
    protected void SetChargeOff()
    {
        ChargingPower = 0;
        chargingVFX.SetActive(true);
        chargingVFX.transform.localScale = Vector3.one * ChargingPower;
    }

    public override void SecondaryFire()
    {
        secondaryFireData.damage = baseSnipeDamage * (chargingPower * chargingPower);
        secondaryFireData.ammoCost = baseSnipeCost * (chargingPower / maxChargePower);
        base.SecondaryFire();
        Shooting(secondaryFireData);

        if(chargingPower >= minChargePower)
        {
            var laser = Instantiate(superLaserVFX).GetComponentInChildren<StatiticLaserVFXManager>();
            laser.multiplierScale = chargingPower;
            laser.SetLaser(gunPointPositions[0].position, GetRayCastMiddle(gunPointPositions[0].position, GetRayRange(secondaryFireData), secondaryFireData.piercingRay));
            Destroy(laser.gameObject, 10f);
        }

        SetChargeOff();
    }

    public override void Shooting(GunFireStruct fireMode, Bullet bulletPrefab = null)
    {
        var target = isPlayerGun ? movimentoMouse.raycastResult : enemyTarget;
        if(!isPlayerGun) transform.LookAt(enemyTarget);
        
        var rigidbodys = GetTargetsOnRange(fireMode.maxDistance);

        if(primaryFireData == fireMode & rigidbodys.Count > 0 & LoadedAmmo > 0)
        {
            LoadedAmmo -= fireMode.ammoCost * Time.deltaTime;
            for (int i = 0; i < gunPointPositions.Length; i++)
            {
                var curPoint = gunPointPositions[i];
                var curTarget = rigidbodys[i % rigidbodys.Count];
                var damageType = fireMode.damageType;
                var targetCenter = curTarget.transform.position + curTarget.centerOfMass;

                Debug.Log("Target Center: " + targetCenter + " // transform = " + curTarget.transform.position);

                var eletricVFX = curPoint.GetComponentInChildren<GunVFXManager>();
                if(eletricVFX != null) eletricVFX.SetLaser(curPoint.position, targetCenter);

                Health targetHealth = curTarget.GetComponentInChildren<Health>();
                if(fireMode.continuosFire) targetHealth?.UpdateHealth(fireMode.damage * Time.deltaTime, damageType);
                else targetHealth?.UpdateHealth(fireMode.damage, damageType);
                targetHealth?.BleedVFX(targetCenter, damageType, fireMode.continuosFire);
            }
        }
        else base.Shooting(fireMode, bulletPrefab);
    }

    protected List<Rigidbody> GetTargetsOnRange(float range)
    {
        var colliders = Physics.OverlapSphere(transform.parent.position + (transform.forward * range * 0.5f), 0.5f * range,
         MovimentoMouse.GetLayers(isPlayerCast: true), QueryTriggerInteraction.Ignore);
        List<Rigidbody> validColliders = new List<Rigidbody>();
        foreach (var col in colliders)
        {
            if(col.attachedRigidbody == null || validColliders.Contains(col.attachedRigidbody)) continue;
            
            if(col.attachedRigidbody.gameObject.GetComponentInChildren<Health>())
            {
                validColliders.Add(col.attachedRigidbody);
            }
        }
        var names = string.Join(" / ", validColliders.Select(r => r.name)); //validColliders.Select(r => r.name);
        Debug.Log("Geting On Trigger... " + colliders.Length);
        Debug.Log("Sorting Valid... " + validColliders.Count + names);
        return validColliders;
    }

    private void OnDrawGizmos() 
    {
        if(transform.parent == null) return;
        Gizmos.color = Color.blue;
        var range = primaryFireData.maxDistance;
        Gizmos.DrawWireSphere(transform.parent.position + (transform.forward * range * 0.5f), range * 0.5f);
    }
}
