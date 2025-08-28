using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform NPCTransform;

    private bool hasSpoken = false;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnd += ResetDialogueState;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= ResetDialogueState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            DialogueManager dm = other.gameObject.GetComponent<DialogueManager>();
            if (dm != null)
            {
                dm.DialogueStart(dialogueStrings, NPCTransform);
                hasSpoken = true;
            }
            else
            {
                Debug.LogError("Can not find DialogueManager component");
            }
        }

    }

    private void ResetDialogueState()
    {
        hasSpoken = false;
        Debug.Log("ResetDialogueState");
    }
}
[System.Serializable]
public class dialogueString
{
    public string text;
    public bool isEnd;

    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Triggered Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;
}
