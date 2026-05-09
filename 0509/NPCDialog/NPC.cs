using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("玩家設定")]
    public PlayerMovement player;
    
    [Header("對話設定")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("過場動畫設定")]
    public UnityEvent onDialogueEnd;

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
        if (player != null) 
        {
            player.isTalking = true; 
        }

        isDialougeActive = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);

        // 改為呼叫準備該句對話的方法
        PrepareCurrentLine(); 
    }

    void NextLine()
    {
        if (isTyping)
            {
                StopAllCoroutines();
                // 直接顯示完整句子
                dialogueText.SetText(dialogueData.lines[dialogueIndex].sentence);
                isTyping = false;

                // 【新增解決方案】：判斷目前這句話是否有勾選自動播放，若有則啟動專屬的等待協程
                if (dialogueData.lines[dialogueIndex].autoProgress)
                {
                    StartCoroutine(WaitAndAutoProgress());
                }
            }
            else if(++dialogueIndex < dialogueData.lines.Length)
                {
                    PrepareCurrentLine();
                }
            else
            {
                EndDialogue();
            }
    }

    // 新增一個獨立的等待協程
    IEnumerator WaitAndAutoProgress()
    {
        yield return new WaitForSeconds(dialogueData.autoProgressDelay);
        NextLine();
    }

    // 新增的方法：用來更新當下這句話的 UI 與啟動打字協程
    void PrepareCurrentLine()
    {
        DialogueLine currentLine = dialogueData.lines[dialogueIndex];

        // 根據當下的資料更新名字和頭像
        nameText.SetText(currentLine.speakerName);
        portraitImage.sprite = currentLine.speakerPortrait;

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        DialogueLine currentLine = dialogueData.lines[dialogueIndex];

        foreach (char letter in currentLine.sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // 檢查當下這句話是否設定為自動播放
        if (currentLine.autoProgress)
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

        if (player != null) 
        {
            player.isTalking = false; 
        }
        
        onDialogueEnd.Invoke();
    }
}