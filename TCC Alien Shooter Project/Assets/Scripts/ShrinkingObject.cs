using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingObject : MonoBehaviour
{
    [SerializeField] private float waitBeforeStart = 0.5f;
    [SerializeField] private float shrinkDownTo = 0.5f;
    [SerializeField] private float shirinkTime = 1f;
    private float curTimerShrink = 0f;
    [SerializeField] private float destroyTimer = 0.5f;
    private Vector3 startScale;
    private bool isDestroying;
    private Light lightComp;
    private float startIntensity;


    private void Start() 
    {
        startScale = transform.localScale;
        lightComp = GetComponent<Light>();
        if(lightComp != null) startIntensity = lightComp.intensity;
    }

    private void Update() 
    {
        waitBeforeStart -= Time.deltaTime;
        if(waitBeforeStart > 0) return;

        transform.localScale = Vector3.Lerp(startScale, startScale * shrinkDownTo, curTimerShrink / shirinkTime);
        curTimerShrink += Time.deltaTime;

        if(lightComp != null)
        {
            lightComp.intensity = Mathf.Lerp(startIntensity, shrinkDownTo, curTimerShrink / shirinkTime);
        }

        if(curTimerShrink >= shirinkTime)
        {
            GameObject.Destroy(this.gameObject, destroyTimer);
        }
    }
}
