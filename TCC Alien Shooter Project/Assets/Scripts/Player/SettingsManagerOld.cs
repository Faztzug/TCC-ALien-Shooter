using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManagerOld : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] private Button closeButton;
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

        if(GameState.GameStateInstance) GameState.OnSettingsUpdated += SettingsHasUpdated;
    }

    public void Close()
    {
        closeButton.onClick.Invoke();
    }
    
    private void UpdateFileData()
    {
        settingsManager.SaveSettings(SettingsData);
        if(GameState.GameStateInstance) GameState.OnSettingsUpdated?.Invoke();
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
        if(GameState.GameStateInstance) GameState.UpdateQuality();
    }

    private void OnDestroy() 
    {
        if(GameState.GameStateInstance) GameState.OnSettingsUpdated -= SettingsHasUpdated;
    }
}
