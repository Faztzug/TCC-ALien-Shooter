using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleRNG : MonoBehaviour
{
    [SerializeField] private Vector3 maxRngVar;
    [SerializeField] private Vector3 minRngVar;
    void Start()
    {
        var vector = new Vector3();
        vector.x = Random.Range(minRngVar.x, maxRngVar.x);
        vector.y = Random.Range(minRngVar.y, maxRngVar.y);
        vector.z = Random.Range(minRngVar.z, maxRngVar.z);
        transform.localScale += vector;
    }
}
