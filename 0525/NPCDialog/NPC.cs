using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Player")]
    public PlayerMovement player;

    [Header("Dialogue")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("Dialogue Events")]
    public UnityEvent onDialogueEnd;

    [Header("UI")]
    public Button skipButton;

    [Header("Input")]
    public string advanceButton = "Interact";
    public KeyCode advanceKey = KeyCode.None;
    [SerializeField] private float dialogueEndCooldown = 0.5f;

    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;
    private bool advanceButtonAvailable = true;
    private int lastAdvanceFrame = -1;
    private float nextDialogueStartTime;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!isDialogueActive || Time.frameCount == lastAdvanceFrame)
            return;

        if (AdvanceInputPressed())
        {
            NextLine();
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive && Time.time >= nextDialogueStartTime;
    }

    public void Interact()
    {
        if (dialogueData == null && !isDialogueActive)
            return;

        if (!isDialogueActive && Time.time < nextDialogueStartTime)
            return;

        if (isDialogueActive && Time.frameCount == lastAdvanceFrame)
            return;

        lastAdvanceFrame = Time.frameCount;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        if (dialogueData == null || dialogueData.lines == null || dialogueData.lines.Length == 0)
            return;

        isDialogueActive = true;
        dialogueIndex = 0;

        if (player != null) player.isTalking = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipAllDialogue);
        }

        PrepareCurrentLine();
    }

    public void SkipAllDialogue()
    {
        StopAllCoroutines();
        EndDialogue();
    }

    void NextLine()
    {
        lastAdvanceFrame = Time.frameCount;
        StopAllCoroutines();

        if (isTyping)
        {
            if (dialogueText != null)
            {
                dialogueText.SetText(dialogueData.lines[dialogueIndex].sentence);
            }
            isTyping = false;

            if (dialogueData.lines[dialogueIndex].autoProgress)
            {
                StartCoroutine(WaitAndAutoProgress());
            }
        }
        else if (++dialogueIndex < dialogueData.lines.Length)
        {
            PrepareCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator WaitAndAutoProgress()
    {
        yield return new WaitForSeconds(dialogueData.autoProgressDelay);
        NextLine();
    }

    void PrepareCurrentLine()
    {
        DialogueLine currentLine = dialogueData.lines[dialogueIndex];

        if (nameText != null) nameText.SetText(currentLine.speakerName);
        if (portraitImage != null) portraitImage.sprite = currentLine.speakerPortrait;

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.SetText("");

        DialogueLine currentLine = dialogueData.lines[dialogueIndex];

        foreach (char letter in currentLine.sentence)
        {
            if (dialogueText != null) dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (currentLine.autoProgress)
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        nextDialogueStartTime = Time.time + dialogueEndCooldown;

        if (player != null) player.isTalking = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipAllDialogue);
        }

        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }
    }

    private bool AdvanceInputPressed()
    {
        bool pressedButton = false;

        if (advanceButtonAvailable && !string.IsNullOrEmpty(advanceButton))
        {
            try
            {
                pressedButton = Input.GetButtonDown(advanceButton);
            }
            catch (System.ArgumentException)
            {
                advanceButtonAvailable = false;
            }
        }

        bool pressedKey = advanceKey != KeyCode.None && Input.GetKeyDown(advanceKey);
        return pressedButton || pressedKey;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (player != null) player.isTalking = false;
        isDialogueActive = false;
        isTyping = false;
    }

    private void OnDestroy()
    {
        if (player != null) player.isTalking = false;
    }
}
