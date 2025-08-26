using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackStates {Idle, Windup, Impact, Cooldown}

public class MeeleFighter : MonoBehaviour
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;

    BoxCollider swordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;


    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    private void Start()
    {
        if(sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();

            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();
            
            DisableAllColliders();

        }
    }



    public AttackStates AttackState { get; private set; }
    bool doCombo;
    int comboCount = 0;

    public bool InAction { get; private set; } = false;
    public bool InCounter { get; set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if(AttackState == AttackStates.Impact || AttackState == AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        AttackState = AttackStates.Windup;



        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null;

        var animState =  animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while(timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if(AttackState == AttackStates.Windup)
            {
                if (InCounter) break;

                if(normalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    AttackState = AttackStates.Impact;
                    EnableHitbox(attacks[comboCount]);
                }
            }
            else if(AttackState == AttackStates.Impact)
            {
                if(normalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    AttackState = AttackStates.Cooldown;
                    DisableAllColliders();
                }
            }
            else if(AttackState == AttackStates.Cooldown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
            }

            
            yield return null;
        }

        
        AttackState = AttackStates.Idle;
        comboCount = 0;
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

        yield return new WaitForSeconds(animState.length * 0.8f);

        InAction = false;
    }

    public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        InAction = true;

        InCounter = true;
        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyStates.Dead);

        var dispVec = opponent.transform.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);
        opponent.transform.rotation = Quaternion.LookRotation(-dispVec);

        animator.CrossFade("CounterAttack", 0.2f);
        opponent.Animator.CrossFade("CounterAttackVictim", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length * 0.8f);

        InCounter = false;
        opponent.Fighter.InCounter = false;

        InAction = false;
    }


    void EnableHitbox(AttackData attack)
    {
        switch(attack.HitboxYoUse)
        {
            case AttackHitbox.Sword:
                swordCollider.enabled = true;
                break;
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            default:
                break;
        }
    }




    void DisableAllColliders()
    {
        if(swordCollider != null)
        {
            swordCollider.enabled = false;
        }
        if(leftHandCollider != null)
        {
            leftHandCollider.enabled = false;
        }
        if(rightHandCollider != null)
        {
            rightHandCollider.enabled = false;
        }
        if(leftFootCollider != null)
        {
            leftFootCollider.enabled = false;
        }
        if(rightFootCollider != null)
        {
            rightFootCollider.enabled = false;
        }
        
    }

    public List<AttackData> Attacks => attacks;

    public bool IsCounterable => AttackState == AttackStates.Windup && comboCount == 0;

}
