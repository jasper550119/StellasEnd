using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro; // 【新增】為了支援 TextMeshPro
using SmallHedge.SoundManager; // 確保引入音效命名空間

public class MainMenu : MonoBehaviour
{
    [Header("載入畫面設定")]
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    [Header("存檔槽 UI 設定")]
    public GameObject SaveSlotPanel; 
    public Text[] SlotTexts;         

    [Header("刪除確認視窗")]
    public GameObject DeleteConfirmPanel; 
    private int slotToDelete = -1;       

    // ==================== 【新功能】音量設定 UI 設定 ====================
    [Header("音量設定 UI 面板")]
    public GameObject VolumeSettingsPanel; 
    // ====================================================================

    // ==================== 新遊戲開場對話設定 ====================
    [Header("新遊戲開場對話設定")]
    public GameObject introDialoguePanel;  // 黑底的對話 UI 面板
    public TMP_Text introDialogueText;     // 黑底面板上的對話文字
    public TMP_Text introNameText;         // 黑底面板上的說話者名字
    public Image introPortraitImage;       // (可選) 黑底面板上的頭像
    public NPCDialogue openingDialogue;    // 開場對話的腳本化物件資料

    private int currentDialogueIndex;
    private bool isIntroTyping;
    private bool isIntroActive;
    // ================================================================

    private void Start()
    {
        if (SaveSlotPanel != null) SaveSlotPanel.SetActive(false);
        if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(false);
        
        // 確保一開始開場對話面板是關閉的
        if (introDialoguePanel != null) introDialoguePanel.SetActive(false); 

        // ==================== 【新功能】初始化音量面板 ====================
        if (VolumeSettingsPanel != null) VolumeSettingsPanel.SetActive(false);
        // ====================================================================
    }

    private void Update()
    {
        // 如果正在播放開場對話，允許玩家點擊滑鼠或按空白鍵推進對話
        if (isIntroActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            NextIntroLine();
        }
    }

    // ==================== 【新功能】音量面板開關邏輯 ====================
    public void OpenVolumeSettingsPanel()
    {
        if (VolumeSettingsPanel != null) VolumeSettingsPanel.SetActive(true);
    }

    public void CloseVolumeSettingsPanel()
    {
        if (VolumeSettingsPanel != null) VolumeSettingsPanel.SetActive(false);
    }
    // ====================================================================

    public void OpenSaveSlotPanel()
    {
        SaveSlotPanel.SetActive(true);
        UpdateSlotUI();
    }

    public void CloseSaveSlotPanel()
    {
        SaveSlotPanel.SetActive(false);
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < SlotTexts.Length; i++)
        {
            int slotIndex = i + 1;
            string path = Path.Combine(Application.persistentDataPath, $"saveData_{slotIndex}.json");
            SlotTexts[i].text = File.Exists(path) ? $"存檔 {slotIndex}" : "- 空存檔 -";
        }
    }

    public void SelectSlotAndPlay(int slotIndex)
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", slotIndex);
        PlayerPrefs.Save();
        
        string path = Path.Combine(Application.persistentDataPath, $"saveData_{slotIndex}.json");
        
        // 判斷是否有存檔 (舊進度)
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            if (!string.IsNullOrEmpty(data.currentSceneName))
            {
                if (PlayerDataManager.instance != null)
                {
                    PlayerDataManager.instance.isFastTraveling = false;
                    PlayerDataManager.instance.isRespawning = false;
                    PlayerDataManager.instance.hasSavedData = false; 
                    PlayerDataManager.instance.isTransitioning = false; 
                    PlayerDataManager.instance.unlockedAreas = data.unlockedAreas;
                    PlayerDataManager.instance.respawnSceneName = data.respawnSceneName;
                    PlayerDataManager.instance.respawnPosition = data.respawnPosition;
                    PlayerDataManager.instance.hasCheckpoint = data.hasCheckpoint;
                    PlayerDataManager.instance.hasBossKey = data.hasBossKey;
                    PlayerDataManager.instance.isBossDefeated = data.isBossDefeated;
                    PlayerDataManager.instance.hasTriggeredBossIntro = data.hasTriggeredBossIntro;
                    PlayerDataManager.instance.shouldLoadSavedGame = true;
                }
                StartCoroutine(LoadSceneAsyncByName(data.currentSceneName)); 
                return; 
            }
        }

        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.ResetForNewGame();
        }

        // 空存檔（新遊戲）的處理邏輯
        if (openingDialogue != null && openingDialogue.lines.Length > 0)
        {
            // 如果有設定開場對話，則啟動對話
            StartIntroDialogue();
        }
        else
        {
            // 防呆：如果沒有設定對話，直接進第一關
            LoadScene(1); 
        }
    }

    private void StartIntroDialogue()
    {
        isIntroActive = true;
        currentDialogueIndex = 0;
        
        if (SaveSlotPanel != null) SaveSlotPanel.SetActive(false); // 隱藏存檔選擇面板
        if (introDialoguePanel != null) introDialoguePanel.SetActive(true); // 顯示全黑面板

        PrepareIntroLine();
    }

    private void PrepareIntroLine()
    {
        DialogueLine currentLine = openingDialogue.lines[currentDialogueIndex];

        if (introNameText != null) introNameText.SetText(currentLine.speakerName);
        if (introPortraitImage != null)
        {
            // 如果沒有頭像，就隱藏 Image 元件
            introPortraitImage.gameObject.SetActive(currentLine.speakerPortrait != null);
            if (currentLine.speakerPortrait != null)
            {
                introPortraitImage.sprite = currentLine.speakerPortrait;
            }
        }

        StartCoroutine(TypeIntroLine());
    }

    private IEnumerator TypeIntroLine()
    {
        isIntroTyping = true;
        introDialogueText.SetText("");
        DialogueLine currentLine = openingDialogue.lines[currentDialogueIndex];

        foreach (char letter in currentLine.sentence)
        {
            introDialogueText.text += letter;
            yield return new WaitForSeconds(openingDialogue.typingSpeed);
        }

        isIntroTyping = false;

        // 檢查是否自動播放下一句
        if (currentLine.autoProgress)
        {
            yield return new WaitForSeconds(openingDialogue.autoProgressDelay);
            NextIntroLine();
        }
    }

    private void NextIntroLine()
    {
        if (isIntroTyping)
        {
            StopAllCoroutines();
            introDialogueText.SetText(openingDialogue.lines[currentDialogueIndex].sentence);
            isIntroTyping = false;

            if (openingDialogue.lines[currentDialogueIndex].autoProgress)
            {
                StartCoroutine(WaitAndAutoProgressIntro());
            }
        }
        else if (++currentDialogueIndex < openingDialogue.lines.Length)
        {
            PrepareIntroLine();
        }
        else
        {
            EndIntroDialogue();
        }
    }

    private IEnumerator WaitAndAutoProgressIntro()
    {
        yield return new WaitForSeconds(openingDialogue.autoProgressDelay);
        NextIntroLine();
    }

    private void EndIntroDialogue()
    {
        StopAllCoroutines();
        isIntroActive = false;
        
        // 對話結束，正式載入關卡 1
        LoadScene(1);
    }

    IEnumerator LoadSceneAsyncByName(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            if (LoadingBarFill != null) LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }
    }

    public void AskDeleteConfirmation(int slotIndex)
    {
        string path = Path.Combine(Application.persistentDataPath, $"saveData_{slotIndex}.json");
        if (File.Exists(path))
        {
            slotToDelete = slotIndex;
            if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(true);
        }
    }

    public void ConfirmDelete()
    {
        if (slotToDelete != -1)
        {
            string path = Path.Combine(Application.persistentDataPath, $"saveData_{slotToDelete}.json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(false);
            slotToDelete = -1;
            UpdateSlotUI(); 
        }
    }

    public void CancelDelete()
    {
        slotToDelete = -1;
        if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(false);
    }

    public void LoadScene(int sceneId) { StartCoroutine(LoadSceneAsync(sceneId)); }
    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }
    }
    public void Quit() { Application.Quit(); }
}
