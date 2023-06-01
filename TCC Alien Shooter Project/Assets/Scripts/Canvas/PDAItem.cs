using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDAItem : Item
{
    [SerializeField] LoreDocument loreText;
    [SerializeField] float readCooldown = 0.2f;
    float timer;
    AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Update() 
    {
        timer -= Time.deltaTime;
    }

    public override void CollectItem(GameObject obj)
    {
        if(timer > 0) return;
        base.CollectItem(obj);
        OpenPDA();
    }

    public override void InteractingWithItem()
    {
        if(timer > 0) OpenPDA();
        else base.InteractingWithItem();
    }

    private void OpenPDA()
    {
        GameState.OpenPDA(loreText);
        collectSound.PlayOn(audioSource);
        timer = readCooldown;
    }
}
