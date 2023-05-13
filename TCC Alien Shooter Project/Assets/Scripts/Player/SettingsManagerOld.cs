using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManagerOld : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] private Toggle mute;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;
    [SerializeField] private TMP_Dropdown quality;
    [SerializeField] private Toggle fps;
    [SerializeField] private Slider sensibilidadeX;
    [SerializeField] private Slider sensibilidadeY;

    private SettingsDataManager settingsManager = new SettingsDataManager();
    private SettingsData settingsData;
    private SettingsData SettingsData {get => settingsData; 
    set {settingsData = value;}}


    private void Start() 
    {
        SettingsData = settingsManager.LoadSettings();
        mute.isOn = SettingsData.mute;
        music.value = SettingsData.musicVolume;
        sfx.value = SettingsData.sfxVolume;
        quality.value = (int)SettingsData.quality;
        fps.isOn = SettingsData.showFPS;
        sensibilidadeX.value = settingsData.sensibilidadeX;
        sensibilidadeY.value = settingsData.sensibilidadeY;

        GameState.OnSettingsUpdated += SettingsHasUpdated;
    }
    private void UpdateFileData()
    {
        settingsManager.SaveSettings(SettingsData);
        GameState.OnSettingsUpdated?.Invoke();
    }
    public void MuteChanged(bool value)
    {
        SettingsData.mute = value;
        UpdateFileData();
    }
    public void MusicChanged(float value)
    {
        SettingsData.musicVolume = value;
        UpdateFileData();
    }
    public void SFXChanged(float value)
    {
        SettingsData.sfxVolume = value;
        UpdateFileData();
    }
    public void QualityChanged(int value)
    {
        SettingsData.quality = (Quality)value;
        UpdateFileData();
    }
    public void FPSChanged(bool value)
    {
        SettingsData.showFPS = value;
        UpdateFileData();
    }
    public void SensibilidadeXChanged(float value)
    {
        SettingsData.sensibilidadeX = value;
        UpdateFileData();
    }
    public void SensibilidadeYChanged(float value)
    {
        SettingsData.sensibilidadeY = value;
        UpdateFileData();
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
