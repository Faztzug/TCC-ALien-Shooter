using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTouch : DamageHealthCollider
{
    [SerializeField] private bool damageBySecond;
    [SerializeField] float invicibilityTime = 0.1f;
    private float time;

    void Start()
    {
        time = invicibilityTime;
    }

    void Update()
    {
        time -= Time.unscaledDeltaTime;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TouchDamage(hit.gameObject);
    }
    void OnCollisionEnter(Collision hit)
    {
        TouchDamage(hit.gameObject);
    }
    void OnTriggerEnter(Collider hit)
    {
        TouchDamage(hit.gameObject);
    }
    void OnTriggerStay(Collider other)
    {
        TouchDamage(other.gameObject, true);
    }

    private void TouchDamage(GameObject hit, bool damageBySecond = false)
    {
        if((hit.CompareTag("Player")) & time < 0 & damageBySecond == this.damageBySecond)
        {
            if(damageBySecond) 
            {
                GetHealth(hit)?.UpdateHealth(damage * Time.deltaTime, DamageType.NULL);
            }
            else
            {
                GetHealth(hit)?.UpdateHealth(damage, DamageType.NULL);
                time = invicibilityTime;
            }
        }

    }
}
