using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;

public class GameState : MonoBehaviour
{
    static public CanvasManager mainCanvas;
    //static public CinemachineFreeLook cinemachineFreeLook;
    public Transform playerTransform;
    public Transform[] playerBodyParts = new Transform[]{};
    static public Transform playerRandomBodyPart => GameStateInstance.playerBodyParts[UnityEngine.Random.Range(0, GameStateInstance.playerBodyParts.Length)];
    static public Transform PlayerTransform => GameStateInstance.playerTransform;
    public bool isPlayerDead = false;
    static public bool IsPlayerDead {
        get => GameStateInstance.isPlayerDead;
        set => GameStateInstance.isPlayerDead = value;
    }
    public static bool isPlayerDashing {get; set;} = false;
    public static bool isGamePaused {get; set;} = false;
    public bool godMode = false;
    static public bool GodMode => GameStateInstance.godMode;
    static public void ToogleGodMode() => GameStateInstance.godMode = !GodMode;
    static public bool isOnCutscene;
    static public bool skipCutscene;
    private Camera mainCamera;
    static public Camera MainCamera { get => gameState.mainCamera; }
    private Camera cutsceneCamera;
    private static GameState gameState;
    [SerializeField] private GameObject GenericAudioSourcePrefab;
    private GameState() { }

    public static GameState GameStateInstance => gameState;
    public SaveData saveData;
    public static SaveData SaveData { get => gameState.saveData; set => gameState.saveData = value; }
    public static SaveManager saveManager = new SaveManager();
    public static Action OnSettingsUpdated;
    public static Action OnCutsceneEnd;

    private void Awake()
    {
        mainCamera = Camera.main;
        //var cutSceneGOCam = GameObject.FindGameObjectWithTag("CutsceneCamera");
        mainCanvas = gameObject.GetComponentInChildren<CanvasManager>();
        gameState = this;
        SaveData = saveManager.LoadGame();
        
        // if(cutSceneGOCam != null)
        // {
        //     cutsceneCamera = cutSceneGOCam.GetComponent<Camera>();
        //     var timelineAnim = cutSceneGOCam.GetComponent<PlayableDirector>();
        //     if(timelineAnim != null && timelineAnim.duration > 1f && !SaveData.jumpCutscene)
        //     {
        //         SetMainCamera();
        //         SetCutsceneCamera();
        //         Debug.Log("duration " + timelineAnim.duration);
        //         StartCoroutine(EndCutsceneOnTime((float)timelineAnim.duration));
        //     }
        //     else
        //     {
        //         Debug.Log("empty clip");
        //         SetCutsceneCamera();
        //         SetMainCamera();
        //         StartCoroutine(EndCutsceneOnTime(1f));
        //     }
        // }
        // else 
        // {
        //     Debug.Log("CAM IS NULL");
        //     StartCoroutine(EndCutsceneOnTime(1f));
        // }

        saveData.jumpCutscene = false;
        UpdateQuality();
        OnSettingsUpdated?.Invoke();
        saveManager.SaveGame(saveData);
    }
    private void Start() 
    {
        var checkpoint = 
        new Vector3(saveData.checkpointPosition[0], saveData.checkpointPosition[1], saveData.checkpointPosition[2]);
        if(checkpoint != Vector3.zero)
        {
            playerTransform.GetComponent<Movimento>().GoToCheckPoint(checkpoint);
        } 
    }

    private void Update()
    {
        if(isOnCutscene && Input.GetButtonDown("Pause"))
        {
            StartCoroutine(EndCutsceneOnTime(0f));
        }
    }

    public static void RestartStage()
    {
        SaveData.jumpCutscene = false;
        saveManager.ResetCheckPointValue(SaveData);
        saveManager.SaveGame(SaveData);
        ReloadScene(0f, false);
    }

    public static string GetSceneName() => SceneManager.GetActiveScene().name;

    public static void ReloadScene(float waitTime, bool jumpCutscene = true)
    {
        var ob = GameStateInstance;
        var sceneName = ob.gameObject.scene.name;
        SaveData.jumpCutscene = jumpCutscene;
        saveManager.SaveGame(SaveData);
        ob.StartCoroutine(ob.LoadSceneCourotine(waitTime, sceneName));
    }

    public static void LoadScene(string sceneName, float waitTime = 0)
    {
        var ob = GameStateInstance;
        ob.StartCoroutine(ob.LoadSceneCourotine(waitTime, sceneName));
    }

    public static void SetCutsceneCamera()
    {
        gameState.mainCamera.gameObject.SetActive(false);
        gameState.cutsceneCamera?.gameObject.SetActive(true);
        isOnCutscene = true;
    }

    public static void SetMainCamera()
    {
        Debug.Log("Set Main Camera");
        gameState.cutsceneCamera?.gameObject.SetActive(false);
        gameState.mainCamera.gameObject.SetActive(true);
        isOnCutscene = false;
        // cinemachineFreeLook.m_YAxisRecentering.RecenterNow();
        // cinemachineFreeLook.m_RecenterToTargetHeading.RecenterNow();
        // cinemachineFreeLook.m_XAxis.m_Recentering.RecenterNow();
        // cinemachineFreeLook.m_YAxis.m_Recentering.RecenterNow();
    }

    public static void SetCheckPoint(Vector3 position)
    {
        SaveData.checkpointPosition = new float[3]{position.x, position.y, position.z};
        saveManager.SaveGame(SaveData);
        Debug.Log(string.Join(", ", SaveData.checkpointPosition));
    }

    public static void UpdateQuality()
    {
        if(QualitySettings.GetQualityLevel() != (int)SaveData.quality)
        {
            QualitySettings.SetQualityLevel((int)SaveData.quality);
        }
    }

    IEnumerator LoadSceneCourotine(float waitTime, string sceneName)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator EndCutsceneOnTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("End Cutscene On Time");
        SetMainCamera();
        OnCutsceneEnd?.Invoke();
        //mainCanvas.PauseGame();
        yield return new WaitForSecondsRealtime(0.1f);
    }

    public static void InstantiateSound(Sound sound, Vector3 position, float destroyTime = 10f)
    {
        var AudioObject = GameObject.Instantiate(gameState.GenericAudioSourcePrefab, position, Quaternion.identity);
        var audioSource = AudioObject.GetComponent<AudioSource>();
        sound.PlayOn(audioSource);
        Destroy(AudioObject, destroyTime);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(1f);
        LoadScene("Cutscene_Final");
    }
}
