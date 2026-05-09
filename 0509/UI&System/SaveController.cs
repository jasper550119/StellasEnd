using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private int currentSlot = 1;

    void Start()
    {
        currentSlot = PlayerPrefs.GetInt("SelectedSaveSlot", 1);
        saveLocation = Path.Combine(Application.persistentDataPath, $"saveData_{currentSlot}.json");

        // 使用協程讀取，確保物件都已經就緒
        StartCoroutine(LoadGameRoutine());
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            currentSceneName = SceneManager.GetActiveScene().name,
            unlockedAreas = PlayerDataManager.instance.unlockedAreas,
            respawnSceneName = PlayerDataManager.instance.respawnSceneName,
            respawnPosition = PlayerDataManager.instance.respawnPosition,
            hasCheckpoint = PlayerDataManager.instance.hasCheckpoint
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"已儲存進度，場景：{saveData.currentSceneName}");
    }

    private IEnumerator LoadGameRoutine()
    {
        // 等待一幀，確保 Player 物件與其餘 Start() 都執行完畢
        yield return null;

        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && PlayerDataManager.instance != null)
            {
                // 優先判定傳送標記 (座標已由 PlayerHP 處理，這裡只負責清空標記)
                if (PlayerDataManager.instance.isFastTraveling)
                {
                    PlayerDataManager.instance.isFastTraveling = false;
                    Debug.Log("已確認快速傳送狀態，清除標記");
                }
                else if (PlayerDataManager.instance.isRespawning)
                {
                    PlayerDataManager.instance.isRespawning = false;
                    Debug.Log("已確認死亡重生狀態，清除標記");
                }
                // 【新增防呆】確保不是「剛走過場景門 (hasSavedData)」，才是真正的從主選單讀檔
                else if (!PlayerDataManager.instance.hasSavedData)
                {
                    player.transform.position = saveData.playerPosition;
                    Debug.Log("正常從主選單讀檔，移動至：" + player.transform.position);
                }
            }

            // 同步其餘資料
            if (PlayerDataManager.instance != null)
            {
                PlayerDataManager.instance.unlockedAreas = saveData.unlockedAreas;
                PlayerDataManager.instance.respawnSceneName = saveData.respawnSceneName;
                PlayerDataManager.instance.respawnPosition = saveData.respawnPosition;
                PlayerDataManager.instance.hasCheckpoint = saveData.hasCheckpoint;
            }

            if (UIManager.instance != null)
            {
                UIManager.instance.RefreshMapButtons();
            }
        }
    }
}