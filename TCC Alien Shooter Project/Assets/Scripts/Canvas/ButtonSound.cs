using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private string buttonName;
    [SerializeField] private Sound clickSound;
    [SerializeField] private Sound hoverSound;
    protected Button button;
    private EventTrigger eventTrigger;
    private AudioSource audioSource;

    protected virtual void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        eventTrigger = GetComponent<EventTrigger>();
        button.onClick.AddListener(PlayClick);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => PlayHover());
        eventTrigger.triggers.Add(entry);
    }

    private void PlayClick() => clickSound.PlayOn(audioSource);
    private void PlayHover() => hoverSound.PlayOn(audioSource);

    private void OnDestroy() 
    {
        button?.onClick.RemoveListener(PlayClick);
        eventTrigger?.triggers.RemoveAll(e => e.eventID == EventTriggerType.PointerEnter);
    }

    void OnValidate()
    {
        if(!string.IsNullOrEmpty(buttonName)) 
        {
            var tmp = GetComponentInChildren<TextMeshProUGUI>();
            if(tmp != null) tmp.text = buttonName;
        }
    }

}
