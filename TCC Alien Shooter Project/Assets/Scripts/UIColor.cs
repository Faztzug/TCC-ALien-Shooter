using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIColor : MonoBehaviour
{
    public Color Color {get => _color; set {_color = value; UpdateColor();}}
    private Color _color;
    private Image image;
    private TextMeshProUGUI text;
    public void UpdateColor()
    {
        if(!image) image = GetComponent<Image>();
        if(!text) text = GetComponent<TextMeshProUGUI>();
        if(image) image.color = Color;
        if(text) text.color = Color;
    }
}
