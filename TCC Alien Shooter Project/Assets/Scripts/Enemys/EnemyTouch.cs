using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTouch : DamageHealthCollider
{
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

    private void TouchDamage(GameObject hit)
    {
        if((hit.CompareTag("Player")) && time < 0)
        {
            GetHealth(hit)?.UpdateHealth(damage, DamageType.NULL);
            time = invicibilityTime;
        }
    }
}
