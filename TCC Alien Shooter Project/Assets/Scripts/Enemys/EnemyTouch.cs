using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTouch : MonoBehaviour
{
    [SerializeField] private float contactDamage;
    private Health hp;
    [SerializeField] float invicibilityTime = 0.1f;
    private float time;

    void Start()
    {
        hp = GetComponent<Health>();
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
            var damage = contactDamage;
            var healthObj = hit.GetComponentInChildren<Health>();
            var curTransform = hit.transform;
            while (healthObj != null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
            }
            healthObj?.UpdateHealth(contactDamage, DamageType.NULL);
            if(healthObj) Debug.Log("T Ouch!");
            time = invicibilityTime;
        }
    }
}
