using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] List<SceneAsset> levels = new List<SceneAsset>();
#endif
    [SerializeField] private List<string> levelsNames = new List<string>();
    [SerializeField] private string level1 = "Level 1";
    [SerializeField] private string test = "SampleScene";
    public SettingsManagerOld settings; 
    public GameObject instructions;
    public GameObject creditos;
    public Button backJogar;
    public Button backSelecionarFase;

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.None;
        Application.targetFrameRate = 30;
    }
    public void StartGame()
    {
        var dificulty = GameState.SaveData.gameDificulty;
        var save = new SaveManager();
        var sav = save.ResetData();
        sav.gameDificulty = dificulty;
        save.SaveGame(sav);
        SceneManager.LoadScene(level1);
    }

    public void LoadLevel(int level)
    {
        if(level >= levelsNames.Count) return;
        SceneManager.LoadScene(levelsNames[level]);
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

#if UNITY_EDITOR
    public void OnAfterDeserialize ( ) => FillScenes ( );
    public void OnBeforeSerialize ( ) => FillScenes ( );
    public void OnValidate ( ) => FillScenes ( );

    private void FillScenes ( )
    {
        levelsNames = levels.Select(l => l.name).ToList();
    }
#endif
}
