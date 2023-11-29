using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] private int nextLevelN = 2;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            if (GameState.SaveData.unlockLevelsTo < nextLevelN)
            {
                GameState.SaveData.unlockLevelsTo = nextLevelN;
                GameState.saveManager.SaveGame(GameState.SaveData);
            }
            GameState.EndLevel();
            GetComponentInChildren<Collider>().enabled = false;
        }
    }
}
