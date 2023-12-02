using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSliderUIFPS : ValueSliderUI
{
    protected int baseValue = 30;
    protected int incrementValue = 15;

    protected override void Start()
    {
        base.Start();
        var setValue = 0;
        var curFPS = (GameState.SettingsData.FPS - baseValue);
        if(curFPS > 0)
        {
            setValue = curFPS / incrementValue;
        }
        slider.value = setValue;
    }

    protected override void UpdateValue(float sliderValue)
    {
        var fps = baseValue + incrementValue * sliderValue;
        int fpsInt = ((int)fps);
        value.text = fpsInt.ToString();
        GameState.SettingsData.FPS = fpsInt;
        Application.targetFrameRate = fpsInt;
        GameState.settingsManager.SaveSettings(GameState.SettingsData);
    }
}
