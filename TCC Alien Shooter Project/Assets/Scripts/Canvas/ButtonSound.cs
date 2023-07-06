using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private Sound clickSound;
    [SerializeField] private Sound hoverSound;
    private Button button;
    private EventTrigger eventTrigger;
    private AudioSource audioSource;

    private void Start() 
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

}
