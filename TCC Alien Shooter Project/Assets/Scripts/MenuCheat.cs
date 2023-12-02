using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCheat : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButtonDown("GodMode"))
        {
            GameState.ToogleGodMode();
            Debug.Log("GOD MODE: " + GameState.GodMode);
            foreach (var button in FindObjectsOfType<LevelButton>())
            {
                button.UpdateState();
            }
        }
    }
}
