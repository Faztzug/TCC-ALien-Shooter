using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wait : MonoBehaviour
{
    [SerializeField] private float tempo;
    [SerializeField] private string Menu;
    void Start()
    {
        StartCoroutine(Intro());

    }


    IEnumerator Intro()
    {
        yield return new WaitForSeconds(tempo);
            SceneManager.LoadScene(Menu);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(Menu);
        }
    }
}
