using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetReticulaIndex(int index)
    {
        image.sprite = gunReticulas[index];
    }

    public void SetReticulaState(ReticulaState reticulaState)
    {
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
    }
}
