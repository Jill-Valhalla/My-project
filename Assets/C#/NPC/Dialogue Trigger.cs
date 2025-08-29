using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private GameObject interactHint;

    private bool hasSpoken = false;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // 确保提示图标初始状态为隐藏
        if (interactHint != null && interactHint.activeSelf)
        {
            interactHint.SetActive(false);
        }
    }

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnd += ResetDialogueState;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= ResetDialogueState;
    }

    private void Update() 
    {
        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !hasSpoken)
        {
            DialogueManager dm = FindObjectOfType<DialogueManager>();
            if (dm != null && !dm.IsDialogueActive)
            {
                dm.DialogueStart(dialogueStrings, NPCTransform);
                hasSpoken = true;
                if (interactHint != null)
                    interactHint.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            if (other.CompareTag("Player") && !hasSpoken)
            {
                isPlayerInRange = true; 
                if (interactHint != null)
                    interactHint.SetActive(true); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; 
            if (interactHint != null)
                interactHint.SetActive(false); 
        }
    }

    private void ResetDialogueState()
    {
        hasSpoken = false;
        Debug.Log("ResetDialogueState");
        if (isPlayerInRange && interactHint != null)
        {
            interactHint.SetActive(true);
        }
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
