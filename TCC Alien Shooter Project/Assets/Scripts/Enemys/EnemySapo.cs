using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySapo : EnemyBiter
{
    [SerializeField] protected Transform modelTrans;
    [SerializeField] protected float jumpHeight = 1f;
    [SerializeField] protected float jumpTime = 0.5f;
    [Range(0.5f, 2f)][SerializeField] float[] jumpRNG = new float[2]{0.5f, 2f};
    protected bool isJumping;
    private bool isMoving => !isBiting & IsMoving();
    private Sequence jumpTween;
    [SerializeField] protected Sound[] jumpSounds;

    protected override void Start()
    {
        base.Start();
        
        //jumpTween.OnComplete(() => jumpTween.Kill(true));
    }

    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        if(isMoving && !isJumping)
        {
            var rng = Random.Range(jumpRNG[0], jumpRNG[1]);
            var height = jumpHeight * rng;
            var time = jumpTime * rng;
            isJumping = true;
            anim.SetTrigger("Jump");
            var soundIndex = UnityEngine.Random.Range(0, jumpSounds.Length);
            jumpSounds[soundIndex].PlayOn(audioSource);

            jumpTween = DOTween.Sequence();
            jumpTween.Append(modelTrans.DOLocalMoveY(height, time / 2).SetEase(Ease.OutSine));
            jumpTween.Append(modelTrans.DOLocalMoveY(0, time / 2).SetEase(Ease.InSine));
            jumpTween.OnComplete(() => isJumping = false);
        }
    }
}
