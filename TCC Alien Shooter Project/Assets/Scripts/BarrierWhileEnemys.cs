using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierWhileEnemys : MonoBehaviour
{
    [SerializeField] List<Health> dependables = new List<Health>();
    
    private void Start() 
    {
        foreach (var health in dependables)
        {
            health.onDeath += CheckDependables;
        }
    }

    private void CheckDependables()
    {
        Debug.Log("Cheking Barrier");
        foreach (var health in dependables)
        {
            if(health != null && health.CurHealth <= 0) health.onDeath -= CheckDependables;
        }
        StartCoroutine(LaterCheck());
    }

    IEnumerator LaterCheck()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Debug.Log("Is all null? " + (dependables.Find(h => h != null) is null));
        if(dependables.Find(h => h != null)) Debug.Log("Is all dead? " + (dependables.Find(h => !h.isDead) is null));

        if (!dependables.Find(h => h != null) | !dependables.Find(h => !h.isDead))
        {
            Debug.Log("DESTROY Barrier");
            Destroy(this.gameObject);
        }
    }
}
