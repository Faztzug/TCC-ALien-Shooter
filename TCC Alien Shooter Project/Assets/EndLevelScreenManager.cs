using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EndLevelScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bodyCountText;
    [HideInInspector] public int nEnemies = 0;
    [HideInInspector] public int nKillEnemies = 0;
    private int curBodyCount = 0;

    public void SetAnim(int enemies, int bodyCount)
    {
        nEnemies = enemies;
        nKillEnemies = bodyCount;
        DOTween.To(() => curBodyCount, x => curBodyCount = x, nKillEnemies, 4f).SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        bodyCountText.text = curBodyCount + " / " + nEnemies;
    }
}
