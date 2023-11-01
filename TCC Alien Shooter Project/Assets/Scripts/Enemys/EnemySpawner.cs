using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool needsTrigger = true;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private GameObject[] enemysPrefabs;
    private List<EnemyIA> enemysList = new List<EnemyIA>();
    private List<GameObject> bodyList = new List<GameObject>();
    [SerializeField] private int maxBodys = 20;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxEnemys = 10;
    [SerializeField] [Range(0f,10f)] private float[] asyncRng = new float[2];
    private float rngTimer = 0;
    [SerializeField] private bool sendEnemyToPlayer;

    void Start()
    {
        if(autoStart) 
        {
            if(TryGetComponent<Collider>(out Collider col)) col.enabled = false;
            StartCoroutine(AsyncUpdate());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Trigger Spawner!");
            StartCoroutine(AsyncUpdate());
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator AsyncUpdate()
    {
        yield return new WaitForSeconds(rngTimer);

        foreach (var e in enemysList.FindAll(e => e.alive == false))
        {
            bodyList.Add(e.gameObject);
        }
        enemysList.RemoveAll(e => e.alive == false);


        if(maxEnemys > enemysList.Count)
        {
            //Debug.Log("SpawnEnemy!");
            int iRngPrefab = Random.Range(0, enemysPrefabs.Length);
            int iRngPos = Random.Range(0, spawnPoints.Length);

            var enemy = Instantiate(enemysPrefabs[iRngPrefab], spawnPoints[iRngPos].position, transform.rotation).GetComponent<EnemyIA>();
            enemy.transform.position = spawnPoints[iRngPos].position;
            enemysList.Add(enemy);
            if (sendEnemyToPlayer) StartCoroutine(SendEnemyToPlayer(enemy));

            if(bodyList.Count > maxBodys)
            {
                var body = bodyList[0];
                bodyList.Remove(body);
                Destroy(body);
            }
        }

        rngTimer = Random.Range(asyncRng[0], asyncRng[1]);
        //Debug.Log("Wait... " + rngTimer);

        StartCoroutine(AsyncUpdate());
    }

    IEnumerator SendEnemyToPlayer(EnemyIA enemy)
    {
        yield return new WaitForSeconds(0);
        enemy.GoToPlayerDirect(true);
    }
}
