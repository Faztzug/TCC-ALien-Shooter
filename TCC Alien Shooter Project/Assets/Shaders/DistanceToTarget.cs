using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToTarget : MonoBehaviour
{
    public Transform target;
    public float activationDistance = 5;
    public float distancePadding = 2;
    public float normalizedDist = 0;
    private Material mat;
    private MeshRenderer mr;
    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;

    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        normalizedDist = 1 - Mathf.Clamp01((dist - distancePadding) / activationDistance);
        mat.SetFloat("_WoobleAmount", normalizedDist);
    }
}
