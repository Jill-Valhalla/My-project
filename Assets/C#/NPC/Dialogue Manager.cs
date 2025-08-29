using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Character Dialogue UI Components")]
    [SerializeField] private TMP_Text characterDialogueText;
    [SerializeField] private Button characterOption1Button;
    [SerializeField] private Button characterOption2Button;

    [Header("Notice Board UI Components")]
    [SerializeField] private TMP_Text noticeBoardText;
    [SerializeField] private Button noticeBoardOption1Button;
    [SerializeField] private Button noticeBoardOption2Button;

    private TMP_Text currentDialogueText;
    private Button currentOption1Button;
    private Button currentOption2Button;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private CameraController playerCameraController;

    [Header("UI References")]
    [SerializeField] private GameObject characterDialogueUI; 
    [SerializeField] private GameObject noticeBoardUI;

    private GameObject currentDialogueUI;

    private List<dialogueString> dialogueList;
    public bool IsDialogueActive { get; private set; }

    [Header("Player")]
    [SerializeField] private PlayerContotller firstPersonController;
    [SerializeField] private MeeleFighter playerMeeleFighter;
    private Transform playerCamera; 

    private int currentDialogueIndex = 0;

    public static DialogueManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        if (characterDialogueUI != null) characterDialogueUI.SetActive(false);
        if (noticeBoardUI != null) noticeBoardUI.SetActive(false);
        //if (dialogueParent != null) dialogueParent.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    public void DialogueStart(List<dialogueString> textToPrint, Transform NPC, DialogueType dialogueType = DialogueType.CharacterDialogue)
    {
        SetDialogueUI(dialogueType);

        switch (dialogueType)
        {
            case DialogueType.CharacterDialogue:
                currentDialogueText = characterDialogueText;
                currentOption1Button = characterOption1Button;
                currentOption2Button = characterOption2Button;
                break;
            case DialogueType.NoticeBoard:
                currentDialogueText = noticeBoardText;
                currentOption1Button = noticeBoardOption1Button;
                currentOption2Button = noticeBoardOption2Button;
                break;
        }

        // 【确保组件不为空】
        if (currentDialogueText == null)
        {
            Debug.LogError("Dialogue text component is not assigned!");
            return;
        }

        IsDialogueActive = true;
        //dialogueParent.SetActive(true);

        firstPersonController.SetMovementEnabled(false);
        if (playerMeeleFighter != null) playerMeeleFighter.enabled = false;

        if (playerCameraController != null)
        {
            playerCameraController.enabled = false; 
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(TurnCameraTowardsNPC(NPC));

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();

        StartCoroutine(PrintDialogue());
    }

    private void SetDialogueUI(DialogueType type)
    {
        
        if (characterDialogueUI != null) characterDialogueUI.SetActive(false);
        if (noticeBoardUI != null) noticeBoardUI.SetActive(false);

        
        switch (type)
        {
            case DialogueType.CharacterDialogue:
                currentDialogueUI = characterDialogueUI;
                break;
            case DialogueType.NoticeBoard:
                currentDialogueUI = noticeBoardUI;
                break;
        }

        if (currentDialogueUI != null)
        {
            currentDialogueUI.SetActive(true);
            //dialogueParent = currentDialogueUI; 
        }
    }

    private void DisableButtons()
    {
        if (currentOption1Button != null) currentOption1Button.interactable = false;
        if (currentOption2Button != null) currentOption2Button.interactable = false;

        if (currentOption1Button != null)
            currentOption1Button.GetComponentInChildren<TMP_Text>().text = "No option";
        if (currentOption2Button != null)
            currentOption2Button.GetComponentInChildren<TMP_Text>().text = "No option";
    }

    private IEnumerator TurnCameraTowardsNPC(Transform NPC)
    {
        Quaternion startRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position);

        float  elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            playerCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }
        playerCamera.rotation = targetRotation;
    }

    private bool optionSeleted = false;

    private IEnumerator PrintDialogue()
    {
        while (currentDialogueIndex < dialogueList.Count)
        {
            dialogueString line = dialogueList[currentDialogueIndex];

            line.startDialogueEvent?.Invoke();

            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));

                if (currentOption1Button != null) currentOption1Button.interactable = true;
                if (currentOption2Button != null) currentOption2Button.interactable = true;

                if (currentOption1Button != null)
                    currentOption1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                if (currentOption2Button != null)
                    currentOption2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;

                if (currentOption1Button != null)
                    currentOption1Button.onClick.RemoveAllListeners();
                if (currentOption2Button != null)
                    currentOption2Button.onClick.RemoveAllListeners();
                if (currentOption1Button != null)
                    currentOption1Button.onClick.AddListener(() => HandeleOptionSelected(line.option1IndexJump));
                if (currentOption2Button != null)
                    currentOption2Button.onClick.AddListener(() => HandeleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSeleted);
            }
            else 
            {
                yield return StartCoroutine(TypeText(line.text));
            }

            line.endDialogueEvent?.Invoke();

            optionSeleted = false;

        }

        DialogueStop();
    }

    private void HandeleOptionSelected(int indexJump)
    {
        optionSeleted = true;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }

    private IEnumerator TypeText(string text)
    {
        if (currentDialogueText != null)
            currentDialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            if (currentDialogueText != null)
                currentDialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (currentDialogueIndex < dialogueList.Count && !dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if (currentDialogueIndex < dialogueList.Count && dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
            yield break; 
        }
        currentDialogueIndex++;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        if (currentDialogueText != null)
            currentDialogueText.text = "";
        //dialogueText.text = "";
        //dialogueParent.SetActive(false);

        //firstPersonController.enabled = true;
        firstPersonController.SetMovementEnabled(true);
        if (playerMeeleFighter != null) playerMeeleFighter.enabled = true;

        if (playerCameraController != null)
        {
            playerCameraController.enabled = true;
        }

        if (currentDialogueUI != null)
        {
            currentDialogueUI.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnDialogueEnd?.Invoke();
        IsDialogueActive = false;
    }

    public static System.Action OnDialogueEnd;

}
