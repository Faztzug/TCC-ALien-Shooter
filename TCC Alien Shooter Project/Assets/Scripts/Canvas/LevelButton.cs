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
        if(GameState.SaveData.unlockLevelsTo < levelIndex) button.enabled = false;
    }

    public void LoadLevelByIndex()
    {
        menuController.LoadLevel(levelIndex);
    }
}
