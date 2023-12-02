using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueSliderUI : MonoBehaviour
{
    protected Slider slider;
    [SerializeField] protected TextMeshProUGUI value;
    protected virtual void Start()
    {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(UpdateValue);
    }

    protected virtual void UpdateValue(float sliderValue)
    {
        value.text = sliderValue.ToString();
    }
}
