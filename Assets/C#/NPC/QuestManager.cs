using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Current Dialogue Stage")]
    public int dialogueStage = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetDialogueStage(int stage)
    {
        dialogueStage = stage;
        Debug.Log("SetDialogueStage called, stage=" + stage);
    }
}
