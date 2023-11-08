using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnQualityPreset : MonoBehaviour
{
    [SerializeField] private List<Quality> qualitiesDeactive = new List<Quality>(1){Quality.Low};

    void Start()
    {
        CheckState();
        GameState.OnSettingsUpdated += CheckState;
    }

    void CheckState()
    {
        if(qualitiesDeactive.Count > 0)
        {
            gameObject.SetActive(!qualitiesDeactive.Contains(GameState.SettingsData.quality));
        }
    }

    private void OnDestroy()
    {
        GameState.OnSettingsUpdated -= CheckState;
    }


}
