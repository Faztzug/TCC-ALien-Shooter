using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float timer = 1f;
    void Start()
    {
        GameObject.Destroy(this.gameObject, timer);
    }
}
