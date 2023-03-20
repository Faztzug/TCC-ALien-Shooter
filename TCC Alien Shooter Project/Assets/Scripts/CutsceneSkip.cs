using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{    
    [SerializeField] private string sceneName;
    [SerializeField] private float timeToAutoSkip = 60f;
    public float cronomether = 0;
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(AutoLoad());
    }
    void Update()
    {
        cronomether += 1 * Time.unscaledDeltaTime;
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
