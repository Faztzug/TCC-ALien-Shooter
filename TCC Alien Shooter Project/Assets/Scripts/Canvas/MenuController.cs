using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string level1 = "Level 1";
    [SerializeField] private string test = "SampleScene";

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.None;
        Application.targetFrameRate = 30;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(level1);
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
