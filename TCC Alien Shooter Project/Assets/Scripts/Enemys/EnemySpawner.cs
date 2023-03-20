using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool needsTrigger = true;
    [SerializeField] private GameObject[] enemysPrefabs;
    [SerializeField] private List<EnemyIA> enemysList;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxEnemys;
    [SerializeField] [Range(1f,10f)] private float[] asyncRng = new float[2];
    private float rngTimer = 0;

    void Start()
    {
        if(!needsTrigger) 
        {
            if(TryGetComponent<Collider>(out Collider col)) col.enabled = false;
            StartCoroutine(AsyncUpdate());
        }
    }

    IEnumerator AsyncUpdate()
    {
        yield return new WaitForSeconds(rngTimer);
        
        enemysList.RemoveAll(e => e.alive == false);


        if(maxEnemys >= enemysList.Count)
        {
            //Debug.Log("SpawnEnemy!");
            int iRngPrefab = Random.Range(0, enemysPrefabs.Length);
            int iRngPos = Random.Range(0, spawnPoints.Length);

            var enemy = Instantiate(enemysPrefabs[iRngPrefab], spawnPoints[iRngPos].position, transform.rotation).GetComponent<EnemyIA>();
            enemysList.Add(enemy);
        }

        rngTimer = Random.Range(asyncRng[0], asyncRng[1]);
        //Debug.Log("Wait... " + rngTimer);

        StartCoroutine(AsyncUpdate());
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.Log("Trigger Spawner!");
            StartCoroutine(AsyncUpdate());
            GetComponent<Collider>().enabled = false;
        }
    }


}
