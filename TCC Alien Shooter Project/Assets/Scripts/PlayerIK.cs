using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    public Transform lookAtObj;
    public Vector3 LookAtRayHit;
    public float weightIKhand;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        if(lookAtObj == null) return;
        //limitando a rotacao da cabeca
        Vector3 frente = transform.forward;
        Vector3 direcaoAlvo = lookAtObj.transform.position - transform.position;
        float angulo = Vector3.Angle(frente, direcaoAlvo);

        if(LookAtRayHit != Vector3.zero)
        {
            anim.SetLookAtPosition(LookAtRayHit);
            anim.SetIKPosition(AvatarIKGoal.RightHand, LookAtRayHit);
        }
        else
        {
            anim.SetLookAtPosition(lookAtObj.position);
            anim.SetIKPosition(AvatarIKGoal.RightHand, lookAtObj.position);
        }
        

        if (angulo < 70 )
        {
            anim.SetLookAtWeight(1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIKhand);
        }
        else
        {
            anim.SetLookAtWeight(1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIKhand);
        }
    }
}
