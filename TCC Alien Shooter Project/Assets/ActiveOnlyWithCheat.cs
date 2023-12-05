using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnlyWithCheat : MonoBehaviour
{
    private void Start()
    {
        UpdateState();
        GameState.onToggleGodMode += UpdateState;
    }

    void UpdateState()
    {
        gameObject.SetActive(GameState.GodMode);
        Debug.Log("Updating State to " + GameState.GodMode);
    }

    private void OnDestroy()
    {
        GameState.onToggleGodMode -= UpdateState;
    }
}
