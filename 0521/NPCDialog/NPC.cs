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

    [Header("UI 按鈕設定")]
    public Button skipButton;

    private int dialogueIndex;
    private bool isTyping, isDialougeActive;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

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
        if (player != null) player.isTalking = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        dialogueIndex = 0;

        // ⭐【核心修正】動態綁定跳過按鈕事件
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners(); // 先清空之前的殘留事件
            skipButton.onClick.AddListener(SkipAllDialogue); // 綁定當前 NPC 的一鍵跳過
        }

        PrepareCurrentLine();
    }

    public void SkipAllDialogue()
    {
        // 停止所有正在進行的打字協程，直接關閉對話
        StopAllCoroutines(); 
        EndDialogue(); 
    }

    void NextLine()
    {
        // 💡【核心修正】移到最上方！只要手動按 E，一律先清除所有正在跑的打字或等待協程
        StopAllCoroutines(); 

        if (isTyping)
        {
            // 直接顯示完整句子
            dialogueText.SetText(dialogueData.lines[dialogueIndex].sentence);
            isTyping = false;

            // 判斷目前這句話是否有勾選自動播放，若有則啟動專屬的等待協程
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
    
    void EndDialogue()
    {
        isDialougeActive = false;
        if (player != null) player.isTalking = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // ⭐【核心修正】對話結束，移除綁定，防呆
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipAllDialogue);
        }

        // 觸發各自獨立的結束事件（Intro 觸發戰鬥 / Outro 觸發死亡）
        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }
    }

    private void OnDisable()
    {
        if (player != null) player.isTalking = false;
        isDialougeActive = false;
        isTyping = false;
    }

    private void OnDestroy()
    {
        if (player != null) player.isTalking = false;
    }
}
