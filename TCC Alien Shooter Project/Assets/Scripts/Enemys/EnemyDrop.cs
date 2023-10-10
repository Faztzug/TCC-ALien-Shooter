using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private int maxDrops = 2;
    [SerializeField] private GameObject[] itens = new GameObject[2];

    [SerializeField] private float[] dropChance = new float[2]; //0 a 100

    private bool droped = false;
    
    public void Drop()
    {
        if(droped) return;
        Vector3 dropPos = transform.position;
        var rigidBody = gameObject.GetComponent<Rigidbody>();
        if(rigidBody != null) dropPos = rigidBody.worldCenterOfMass;
        dropPos.y += 1.5f;
        Quaternion dropRot = transform.rotation;
        for (int i = 0; i < itens.Length; i++)
        {
            float rng = UnityEngine.Random.Range(0,100);
            if(rng <= dropChance[i])
            {
                var item = Instantiate(itens[i], dropPos, dropRot);
                item.GetComponent<Rigidbody>().AddForce(transform.up * 2f, ForceMode.Impulse);

                Debug.Log("drop parent pos = " + transform.position);
                Debug.Log("drop item pos = " + item.transform.position);
                Debug.Log("expected pos = " + (transform.position + new Vector3(0,1.5f,0)));
                if (i >= maxDrops) return;
            } 
        }
        droped = true;
    }

    private void OnValidate() 
    {
        for (int i = 0; i < dropChance.Length; i++)
        {
            if(dropChance[i] < 0f) dropChance[i] = 0f;
            if(dropChance[i] > 100f) dropChance[i] = 100f;
        }
    }
}
