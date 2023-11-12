using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : ButtonSound
{
    [SerializeField] private int levelIndex;
    private MenuController menuController;

    protected override void Start()
    {
        base.Start();
        menuController = FindObjectOfType<MenuController>(true);
        UpdateState();
    }

    public void UpdateState()
    {
        button.enabled = (GameState.SaveData.unlockLevelsTo >= levelIndex || GameState.GodMode);
    }

    public void LoadLevelByIndex()
    {
        menuController.LoadLevel(levelIndex);
    }
}
