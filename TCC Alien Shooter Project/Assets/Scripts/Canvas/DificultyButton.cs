using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DificultyButton : ButtonSound
{
    [SerializeField] private GameDificulty gameDificulty = GameDificulty.Normal;
    private Color normalColor;
    private Color selectedColor;

    protected override void Start()
    {
        base.Start();
        if(GameState.SaveData.gameDificulty == gameDificulty) button.Select();
        normalColor = button.colors.normalColor;
        selectedColor = button.colors.selectedColor;
    }
    void OnEnable()
    {
        if(button != null & GameState.SaveData.gameDificulty == gameDificulty) button.Select();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == button.gameObject) return;
        var colorBlock = button.colors;
        colorBlock.normalColor = GameState.SaveData.gameDificulty == gameDificulty ? selectedColor : normalColor;
        button.colors = colorBlock;
    }

    public void SetDificulty()
    {
        GameState.SaveData.gameDificulty = gameDificulty;
        GameState.SaveGameData();
        Debug.Log("Saved to " + gameDificulty.ToString() + " end result: " + GameState.SaveData.gameDificulty);
        button.Select();
    }
}
