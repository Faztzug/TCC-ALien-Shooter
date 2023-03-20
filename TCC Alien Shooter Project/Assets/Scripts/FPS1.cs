using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS1 : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private int framesCount = 60;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        InvokeRepeating(nameof(CalcularFPS), 0, 1f);
    }

    private void Update() 
    {
        framesCount++;
    }

    private void CalcularFPS()
    {
        textMesh.text = framesCount.ToString("FPS 00");
        framesCount = 0;
    }
}
