using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private int currentSlot = 1;

    void Start()
    {
        // 從 PlayerPrefs 讀取玩家在主選單選擇的存檔編號 (預設為 1)
        currentSlot = PlayerPrefs.GetInt("SelectedSaveSlot", 1);
        
        // 動態生成存檔路徑，例如 "saveData_1.json", "saveData_2.json"
        saveLocation = Path.Combine(Application.persistentDataPath, $"saveData_{currentSlot}.json");

        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            // 提醒：你原本的 SaveData 裡有一個 mapBounddary 變數，若有用到記得在這裡賦值
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"已儲存進度至存檔槽 {currentSlot}");
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            Debug.Log($"已讀取存檔槽 {currentSlot}");
        }
        else
        {
            // 如果檔案不存在，代表是新遊戲，建立初始存檔
            SaveGame();
        }
    }
}