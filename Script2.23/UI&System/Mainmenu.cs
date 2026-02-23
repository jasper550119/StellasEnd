using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("載入畫面設定")]
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    [Header("存檔槽 UI 設定")]
    public GameObject SaveSlotPanel; 
    public Text[] SlotTexts;         

    [Header("刪除確認視窗")]
    public GameObject DeleteConfirmPanel; // 確認視窗的物件
    private int slotToDelete = -1;       // 暫時記錄要刪除哪一個存檔

    private void Start()
    {
        if (SaveSlotPanel != null) SaveSlotPanel.SetActive(false);
        if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(false);
    }

    // --- 原有的存檔控制方法 ---

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
        LoadScene(1); 
    }

    // 當玩家點擊存檔旁邊的「刪除」按鈕時呼叫
    public void AskDeleteConfirmation(int slotIndex)
    {
        // 1. 先組合出路徑，確認檔案到底在不在
        string path = Path.Combine(Application.persistentDataPath, $"saveData_{slotIndex}.json");

        // 2. 判斷檔案是否存在
        if (File.Exists(path))
        {
            // 檔案存在：記錄編號並跳出確認視窗
            slotToDelete = slotIndex;
            if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(true);
            Debug.Log($"存檔 {slotIndex} 存在，準備詢問是否刪除。");
        }
        else
        {
            // 檔案不存在：直接無視，不跳視窗
            Debug.Log($"存檔 {slotIndex} 是空的，無視刪除請求。");
        }
    }

    // 3. 當玩家點擊「取消」時呼叫
    public void CancelDelete()
    {
        slotToDelete = -1; // 清空暫存
        if (DeleteConfirmPanel != null) DeleteConfirmPanel.SetActive(false); // 隱藏確認視窗
    }

    // --- 場景載入與離開 ---
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