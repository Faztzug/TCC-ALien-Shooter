using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{ 
    [SerializeField] protected int ammount;
    [SerializeField] protected Sound collectSound;

    protected virtual void Start() { }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("On Item TRigger 0");
            CollectItem(other.gameObject);
        }

    }

    public virtual void CollectItem(GameObject obj)
    {
        Debug.Log("Collect Item 1 " + obj.name);
    }

    public virtual void InteractingWithItem()
    {
        Debug.Log("Interact with Item 0");
        CollectItem(GameState.PlayerTransform.gameObject);
    }

    public virtual void DestroyItem()
    {
        Debug.Log("Destroy Item 2");
        if(collectSound.clip != null) GameState.InstantiateSound(collectSound, transform.position);
        this.gameObject.SetActive(false);
    }
}
