using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField] private float gravity = 1f;
    private float gravityAcceleration;
    private Rigidbody rgbd;

    private void Start() 
    {
        rgbd = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        gravityAcceleration -= gravity * Time.deltaTime;
        var gravityVector = rgbd.position;
        gravityVector.y += gravityAcceleration;
        rgbd.velocity = gravityVector;
    }
}
