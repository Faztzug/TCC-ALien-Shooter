using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{ 
    [SerializeField] protected int ammount;
    [SerializeField] protected Sound collectSound;

    protected virtual void Start() { }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }

    }

    public virtual void CollectItem(GameObject obj)
    {

    }

    public virtual void InteractingWithItem()
    {
        CollectItem(GameState.PlayerTransform.gameObject);
    }

    public virtual void DestroyItem()
    {
        if(collectSound.clip != null) GameState.InstantiateSound(collectSound, transform.position);
        this.gameObject.SetActive(false);
    }
}
