using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTriggerTest : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueLines = new List<dialogueString>();
    [SerializeField] private GameObject interactHint;

    [Header("Dialogue Lists for Stages")]
    public List<dialogueString> dialogueStage0;
    public List<dialogueString> dialogueStage2;
    //public List<dialogueString> dialogueLines;

    private bool hasSpoken = false;
    private bool isPlayerInRange = false;

    [Header("End Dialogue Event")]
    public UnityEvent endDialogueEvent;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)&& !DialogueManager.Instance.IsDialogueActive)
        {
            
            if (CanTriggerDialogue())
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        hasSpoken = true;
        if (interactHint != null)
            interactHint.SetActive(false);

        List<dialogueString> linesToShow = dialogueLines;

        if (gameObject.name == "A")
        {
            if (QuestManager.Instance.dialogueStage == 0)
                linesToShow = dialogueStage0;
            else if (QuestManager.Instance.dialogueStage == 2)
                linesToShow = dialogueStage2;
        }

        if (linesToShow != null && linesToShow.Count > 0)
        {
            DialogueManager.Instance.DialogueStart(linesToShow, transform, DialogueType.CharacterDialogue);
        }

        endDialogueEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactHint != null)
                interactHint.SetActive(true);
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

    private bool CanTriggerDialogue()
    {
        // A NPC
        if (gameObject.name == "A")
        {
            return QuestManager.Instance.dialogueStage == 0 || QuestManager.Instance.dialogueStage == 2;
        }
        // B NPC
        else if (gameObject.name == "B")
        {
            return QuestManager.Instance.dialogueStage == 1;
        }
        return true;
    }
}
