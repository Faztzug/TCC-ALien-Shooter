using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDAItem : Item
{
    [SerializeField] LoreDocument loreText;
    [SerializeField] float readCooldown = 0.2f;
    float timer;
    AudioSource audioSource;
    [SerializeField] bool countTowardsEnd = true;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponentInChildren<AudioSource>();
        if (countTowardsEnd & !GameState.allPdasOnLevel.Contains(loreText.tittleText)) GameState.allPdasOnLevel.Add(loreText.tittleText);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);
        //do nothing
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
        if(countTowardsEnd & !GameState.allPdasfound.Contains(loreText.tittleText)) GameState.allPdasfound.Add(loreText.tittleText);
        GameState.OpenPDA(loreText);
        collectSound.PlayOn(audioSource);
        timer = readCooldown;
    }
}
