using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EndLevelScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bodyCountText;
    [SerializeField] private TextMeshProUGUI pdaCountText;
    [HideInInspector] public int nEnemies = 0;
    [HideInInspector] public int nKillEnemies = 0;
    [HideInInspector] public int nAllPDA = 0;
    [HideInInspector] public int nFoundPDA = 0;
    private int curBodyCount = 0;
    private int curPdaCount = 0;

    public void SetAnim(int enemies, int bodyCount, int pdas, int found)
    {
        nEnemies = enemies;
        nKillEnemies = bodyCount;
        nAllPDA = pdas;
        nFoundPDA = found;
        var music = FindObjectOfType<MusicPlayer>();
        music.GetAudioSource.DOFade(0f, 0.2f).OnComplete(music.GetAudioSource.Stop);
        GetComponentInChildren<IddleSound>()?.Play();
        DOTween.To(() => curBodyCount, x => curBodyCount = x, nKillEnemies, 7f).SetEase(Ease.InOutSine);
        DOTween.To(() => curPdaCount, x => curPdaCount = x, nFoundPDA, 7f).SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        bodyCountText.text = curBodyCount + " / " + nEnemies;
        pdaCountText.text = curPdaCount + " / " + nAllPDA;
    }
}
