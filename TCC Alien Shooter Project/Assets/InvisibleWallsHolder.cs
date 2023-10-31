using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsHolder : MonoBehaviour
{
    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) renderer.enabled = false;
    }
}
