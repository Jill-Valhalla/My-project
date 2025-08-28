
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public EnemyController targetEnemy;

    MeeleFighter meeleFighter;
    CameraController cam;

    private void Awake()
    {
        meeleFighter = GetComponent<MeeleFighter>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (Input.GetButtonDown("Attack"))
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if (enemy != null && enemy.Fighter.IsCounterable && !meeleFighter.InAction)
            {
                StartCoroutine(meeleFighter.PerformCounterAttack(enemy));

            }
            else
            {
                meeleFighter.TryToAttack();
            }

                
        }
    }

    public Vector3 GetTargetingDir()
    {
        var vecFromCam = transform.position - cam.transform.position;
        vecFromCam.y = 0f;
        return vecFromCam.normalized;
    }
}
