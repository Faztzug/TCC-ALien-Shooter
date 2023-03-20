using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticulaFeedback : MonoBehaviour
{
    //[SerializeField] private MenuPause canvasScript;
    [SerializeField] private Image reticula;
    [SerializeField] private Sprite neutralState;
    [SerializeField] private Sprite enemyState;
    private bool onEnemy;
    void Start()
    {
        reticula.sprite = neutralState;
        onEnemy = false;
        //canvasScript.player.GetComponent<Movimento>().reticula = this;
    }

    public void NeutralState()
    {
        if(onEnemy == false) return;
        reticula.sprite = neutralState;
        onEnemy = false;
    }
    public void EnemyState()
    {
        if(onEnemy == true) return;
        reticula.sprite = enemyState;
        onEnemy = true;
    }
}
