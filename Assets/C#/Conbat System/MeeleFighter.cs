using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

//public enum AttackState {Idle, Windup, Impact, Cooldown}

public class MeeleFighter : MonoBehaviour
{
    
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        animator.CrossFade("Slash", 0.2f);
        yield return null;

        var animState =  animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length);

        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hitbox" && !InAction)
        {
             StartCoroutine(PlayHitReaction());
        }
    }

    IEnumerator PlayHitReaction()
    {
        InAction = true;

        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length);

        InAction = false;
    }


}
