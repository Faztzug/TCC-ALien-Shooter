using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager
{
    private const string kSavePath = "/MySaveData.dat";
    public void SaveGame(SaveData saveData)
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + kSavePath); 
        bf.Serialize(file, saveData);
        file.Close();
        //Debug.Log("Game data saved!");
    }
    public SaveData LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + kSavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + kSavePath, FileMode.Open);
            SaveData loadData = (SaveData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
            return loadData;
        }
        else
        {
            Debug.LogError("There is no save data!");
            return new SaveData();
        }
    }
    
    public SaveData ResetData()
    {
        if (File.Exists(Application.persistentDataPath + kSavePath))
        {
            File.Delete(Application.persistentDataPath + kSavePath);
            Debug.Log("Data reset complete!");
        }
        else
        Debug.LogError("No save data to delete.");
        return new SaveData();
    }

    public SaveData ResetCheckPointValue(SaveData saveData)
    {
        saveData.checkpointPosition = new float[3]{0,0,0};
        saveData.animalColetadoNaFase = false;
        saveData.plantaColetadaNaFase = false;
        SaveGame(saveData);
        return LoadGame();
    }
}

[Serializable]
public enum Quality
{
    Low,
    Medium,
    High,
}

[Serializable]
public class SaveData
{
    [Header("Settings")]
    public bool mute;
    public float sfxVolume;
    public float musicVolume;
    public Quality quality;
    public bool showFPS;

    [Header("GameData")]
    public int unlockLevelsTo = 1;
    public bool heliconiaColetada;
    public bool oncaColetada;
    public bool planta2;
    public bool animal2;
    public bool planta3;
    public bool animal3;
    public float[] checkpointPosition = new float[3]{0,0,0};
    public bool animalColetadoNaFase;
    public bool plantaColetadaNaFase;

    [Header("Cutscene")]
    public bool jumpCutscene;

    public SaveData()
    {
        mute = false;
        musicVolume = 1f;
        sfxVolume = 1f;
        quality = Quality.High;
        showFPS = true;

        unlockLevelsTo = 1;
        checkpointPosition = new float[3]{0,0,0};
    }
}
