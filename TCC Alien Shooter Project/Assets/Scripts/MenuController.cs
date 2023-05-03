using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private void Start() 
    {
        Cursor.lockState = CursorLockMode.None;
        Application.targetFrameRate = 30;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }
    
    public void TestLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Options()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
