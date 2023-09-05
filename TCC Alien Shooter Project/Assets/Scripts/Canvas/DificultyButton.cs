using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class DificultyButton : ButtonSound
{
    [SerializeField] private GameDificulty gameDificulty = GameDificulty.Normal;
    protected override void Start()
    {
        base.Start();
        if(GameState.SaveData.gameDificulty == gameDificulty) button.Select();
    }
    void OnEnable()
    {
        if(button != null & GameState.SaveData.gameDificulty == gameDificulty) button.Select();
    }

    public void SetDificulty()
    {
        GameState.SaveData.gameDificulty = gameDificulty;
        GameState.SaveGameData();
        Debug.Log("Saved to " + gameDificulty.ToString() + " end result: " + GameState.SaveData.gameDificulty);
        button.Select();
    }
}
