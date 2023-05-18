using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDAItem : Item
{
    [SerializeField] LoreDocument loreText;

    public override void CollectItem(Collider info)
    {
        base.CollectItem(info);
        GameState.OpenPDA(loreText);
    }
}
