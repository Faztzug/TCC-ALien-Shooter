using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDummy : EnemyIA
{
    protected override void Update() 
    {
        base.Update();
    }

    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();

        base.GoToPlayerOffset();
    }
}
