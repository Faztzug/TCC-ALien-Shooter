using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ReticulaState
{
    Neutral,
    Enemy,
    Interactable,
}
public class ReticulaFeedback : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] gunReticulas;
    [SerializeField] private UIColor uIColor;
    [SerializeField] private Color enemyColor;
    [SerializeField] private Color interactableColor;
    [SerializeField] private TextMeshProUGUI interactableText;
    private ReticulaState curState;

    private void Start()
    {
        interactableText.color = interactableColor;
    }

    private void Update() 
    {
        var flag = curState == ReticulaState.Interactable && GameState.MovimentoMouse.isOnInteractableDistance;
        interactableText.gameObject.SetActive(flag);
    }

    public void SetReticulaIndex(int index)
    {
        image.sprite = gunReticulas[index];
    }

    public void SetReticulaState(ReticulaState reticulaState, string middleScreenText = null)
    {
        curState = reticulaState;
        switch (reticulaState)
        {
            case ReticulaState.Enemy :
                image.color = enemyColor;
                break;
            case ReticulaState.Interactable :
                image.color = interactableColor;
                break;
            default: 
                image.color = uIColor.Color;
                break;
        }
        var prefix = "E - ";
        if (middleScreenText == null) middleScreenText = "Interagir";
        interactableText.text = prefix + middleScreenText;
    }
}
