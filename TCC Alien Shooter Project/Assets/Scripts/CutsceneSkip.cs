using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{    
    [SerializeField] private string sceneName;
    [SerializeField] private float timeToAutoSkip = 60f;
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(AutoLoad());
    }
    void Update()
    {
        if(Input.GetButtonUp("Pause")) LoadNextScene();
    }
    IEnumerator AutoLoad()
    {
        yield return new WaitForSecondsRealtime(timeToAutoSkip);

        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
