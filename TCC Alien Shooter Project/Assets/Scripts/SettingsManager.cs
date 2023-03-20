using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] private Toggle mute;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;
    [SerializeField] private TMP_Dropdown quality;
    [SerializeField] private Toggle fps;
    private SaveManager saveManager = new SaveManager();


    private void Start() 
    {
        var data = GameState.SaveData;
        mute.isOn = data.mute;
        music.value = data.musicVolume;
        sfx.value = data.sfxVolume;
        quality.value = (int)data.quality;
        fps.isOn = data.showFPS;

        GameState.OnSettingsUpdated += SettingsHasUpdated;
    }
    public void UpdateSave()
    {
        saveManager.SaveGame(GetSaveData());
        GameState.OnSettingsUpdated?.Invoke();
    }
    public void MuteChanged(bool value)
    {
        GetSaveData().mute = value;
    }
    public void MusicChanged(float value)
    {
        GetSaveData().musicVolume = value;
    }
    public void SFXChanged(float value)
    {
        GetSaveData().sfxVolume = value;
    }
    public void QualityChanged(int value)
    {
        GetSaveData().quality = (Quality)value;
    }
    public void FPSChanged(bool value)
    {
        GetSaveData().showFPS = value;
    }

    private SaveData GetSaveData()
    {
        return GameState.SaveData;
    }

    private void SettingsHasUpdated()
    {
        GameState.UpdateQuality();
    }

    private void OnDestroy() 
    {
        GameState.OnSettingsUpdated -= SettingsHasUpdated;
    }
}
