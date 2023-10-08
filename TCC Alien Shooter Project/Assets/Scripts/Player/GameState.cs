using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    static public CanvasManager mainCanvas;
    //static public CinemachineFreeLook cinemachineFreeLook;
    public Transform playerTransform;
    public Transform playerMiddleT;
    public Transform[] playerBodyParts = new Transform[]{};
    static public Transform playerRandomBodyPart => GameStateInstance.playerBodyParts[UnityEngine.Random.Range(0, GameStateInstance.playerBodyParts.Length)];
    static public Transform PlayerTransform => GameStateInstance.playerTransform;
    static public Transform PlayerMiddleT => GameStateInstance.playerMiddleT;
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
    private MovimentoMouse movimentoMouse;
    static public MovimentoMouse MovimentoMouse { get => gameState.movimentoMouse; }
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
    public SettingsData settingsData;
    public static SettingsData SettingsData { get => gameState.settingsData; set => gameState.settingsData = value; }
    public static SettingsDataManager settingsManager = new SettingsDataManager();

    public static Action OnSettingsUpdated;
    public static Action OnCutsceneEnd;
    
    //[SerializeField] private string nextScene = "MenuInicial";
    [SerializeField] private GameObject endCanvas;
    [HideInInspector] public static int nEnemies = 0;
    [HideInInspector] public static int nKillEnemies = 0;
    private DateTime levelStartTime = DateTime.Now;

    private void Awake()
    {
        mainCamera = Camera.main;
        movimentoMouse = GetComponentInChildren<MovimentoMouse>();
        //var cutSceneGOCam = GameObject.FindGameObjectWithTag("CutsceneCamera");
        mainCanvas = gameObject.GetComponentInChildren<CanvasManager>();
        gameState = this;
        SaveData = saveManager.LoadGame();
        if(SaveData.gunsColected == null) SaveData = saveManager.ResetData();
        
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
        OnSettingsUpdated += ReloadSettings;
        OnSettingsUpdated?.Invoke();
        saveManager.SaveGame(saveData);
        PauseGame(false);
    }

    static public void SaveGameData() => saveManager.SaveGame(SaveData);

    static public void ReloadSettings()
    {
        Debug.Log("RELOADING SETTINGS");
        SettingsData = settingsManager.LoadSettings();
    }
    private void OnDestroy() 
    {
        OnSettingsUpdated -= ReloadSettings;
    }

    private void Start() 
    {
        var checkpoint = new Vector3(saveData.checkpointPosition[0], 
        saveData.checkpointPosition[1], saveData.checkpointPosition[2]);
        if(checkpoint != Vector3.zero)
        {
            playerTransform.GetComponent<Movimento>().GoToCheckPoint(checkpoint);
        }
        levelStartTime = DateTime.Now;
    }

    private void Update()
    {
        if(isOnCutscene && Input.GetButtonDown("Pause"))
        {
            StartCoroutine(EndCutsceneOnTime(0f));
        }
        else if(Input.GetButtonDown("Pause")) PauseGame(!isGamePaused);
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
    }

    public static void SetCheckPoint(Vector3 position)
    {
        SaveData.checkpointPosition = new float[3]{position.x, position.y, position.z};
        saveManager.SaveGame(SaveData);
        Debug.Log(string.Join(", ", SaveData.checkpointPosition));
    }

    public static void UpdateQuality()
    {
        if(QualitySettings.GetQualityLevel() != (int)SettingsData.quality)
        {
            QualitySettings.SetQualityLevel((int)SettingsData.quality);
        }
    }

    IEnumerator LoadSceneCourotine(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);

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

    public static void PauseGame(bool pause)
    {
        if(mainCanvas == null)
        {
            Debug.Log("On main menu Pause Action");
            var mainMenu = FindFirstObjectByType<MenuController>(FindObjectsInactive.Include);
            if(mainMenu.settings.gameObject.activeInHierarchy) mainMenu.settings.Close();
            else if(mainMenu.instructions.activeInHierarchy) mainMenu.instructions.GetComponentInChildren<Button>().onClick.Invoke();
            else if(mainMenu.creditos.activeInHierarchy) mainMenu.creditos.GetComponentInChildren<Button>().onClick.Invoke();
            else if(mainMenu.backJogar.gameObject.activeInHierarchy) mainMenu.backJogar.onClick.Invoke();
            else if(mainMenu.backSelecionarFase.gameObject.activeInHierarchy) mainMenu.backSelecionarFase.onClick.Invoke();
            Cursor.lockState = CursorLockMode.None;
            isGamePaused = false;
            return;
        }

        if(isGamePaused && mainCanvas != null && !mainCanvas.DoesExitPause())
        {
            Debug.Log("should exit pause? " + mainCanvas.DoesExitPause());
            mainCanvas.SetSettingsMenu(false);
            mainCanvas.SetTutorial(false);
            mainCanvas.SetPDAdocument(false);
            mainCanvas.SetPauseMenu(true);
            Cursor.lockState = isGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
            return;
        }
        isGamePaused = pause;
        Cursor.lockState = isGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pause ? 0f : 1f;
        mainCanvas?.SetPauseMenu(pause);
        mainCanvas?.SetPDAdocument(false);
    }
    
    public static void OpenPDA(LoreDocument loreText)
    {
        PauseGame(true);
        mainCanvas.SetPauseMenu(false);
        mainCanvas.SetSettingsMenu(false);
        mainCanvas.SetTutorial(false);
        mainCanvas.SetPDAdocument(true, loreText);
    }

    public static void EndLevel()
    {
        if(GodMode) ToogleGodMode();
        gameState.StartCoroutine(gameState.EndLevelCourotine());
    }
    IEnumerator EndLevelCourotine()
    {
        var totalTime = DateTime.Now - levelStartTime;
        var go = GameObject.Instantiate(endCanvas,null);
        go.SetActive(true);
        var endLevelScreen = go.GetComponentInChildren<EndLevelScreenManager>();
        endLevelScreen?.SetAnim(nEnemies, nKillEnemies);
        yield return new WaitForSecondsRealtime(5f);
        var nextSceneI = this.gameObject.scene.buildIndex + 1;
        if (nextSceneI >= SceneManager.sceneCountInBuildSettings) nextSceneI = 0;
        /*Debug.Log("cur scene I " + this.gameObject.scene.buildIndex);
        Debug.Log("NEXT scene I " + nextSceneI);
        Debug.Log("NEXT scene is valid? " + SceneManager.GetSceneByBuildIndex(nextSceneI).IsValid());
        Debug.Log("NEXT scene name " + SceneManager.GetSceneByBuildIndex(nextSceneI).name);*/


        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneI);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        GameState.LoadScene(sceneName);
    }
}
