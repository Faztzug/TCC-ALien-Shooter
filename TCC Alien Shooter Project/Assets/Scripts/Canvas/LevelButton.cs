using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : ButtonSound
{
    [SerializeField] private int levelIndex;
    private MenuController menuController;
    [SerializeField] private Sprite levelImageOn;
    [SerializeField] private Sprite levelImageOff;
    [SerializeField] private Image image;

    protected override void Start()
    {
        base.Start();
        menuController = FindObjectOfType<MenuController>(true);
        UpdateState();
    }

    public void UpdateState()
    {
        button.enabled = (GameState.SaveData.unlockLevelsTo >= levelIndex || GameState.GodMode);
        image.sprite = button.enabled ? levelImageOn : levelImageOff;
    }

    public void LoadLevelByIndex()
    {
        menuController.LoadLevel(levelIndex);
    }

    private void OnValidate()
    {
        if(image != null) image.sprite = levelImageOn;
    }
}
