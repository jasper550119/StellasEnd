using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("對話設定")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("過場動畫設定")]
    public UnityEvent onDialogueEnd; // 新增這行：對話結束事件

    private int dialogueIndex;
    private bool isTyping, isDialougeActive;

    public bool CanInteract()
    {
        return !isDialougeActive;
    }

    public void Interact()
    {
        if (dialogueData == null && !isDialougeActive)
            return;

        if (isDialougeActive)
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
        isDialougeActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLine.Length > dialogueIndex && dialogueData.autoProgressLine[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialougeActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        onDialogueEnd.Invoke(); // 新增這行：觸發結束事件
    }
}

