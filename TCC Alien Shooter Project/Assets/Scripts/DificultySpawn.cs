using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DificultySpawn : MonoBehaviour
{
    [SerializeField] List<GameDificulty> spawnOnThoseDificulties;
    void Start()
    {
        if(!spawnOnThoseDificulties.Contains(GameState.SaveData.gameDificulty)) this.gameObject.SetActive(false);
    }
}
