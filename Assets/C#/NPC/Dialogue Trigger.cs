using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public enum DialogueType
{
    CharacterDialogue,
    NoticeBoard
}

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private GameObject interactHint;
    [SerializeField] private DialogueType dialogueType = DialogueType.CharacterDialogue;

    private bool hasSpoken = false;
    private bool isPlayerInRange = false;

    private void Start()
    {
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
            if (CheckDialogueStage())
            {
                DialogueManager dm = FindObjectOfType<DialogueManager>();
                if (dm != null && !dm.IsDialogueActive)
                {
                    dm.DialogueStart(dialogueStrings, NPCTransform, dialogueType);
                    hasSpoken = true;
                    if (interactHint != null)
                        interactHint.SetActive(false);
                }
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

    private bool CheckDialogueStage()
    {
        
        if (gameObject.name == "A_NPC")
        {
            return QuestManager.Instance.dialogueStage == 0 || QuestManager.Instance.dialogueStage == 2;
        }
       
        else if (gameObject.name == "B_NPC")
        {
            return QuestManager.Instance.dialogueStage == 1;
        }
        return true;
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

