using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private int currentSlot = 1;

    void Start()
    {
        currentSlot = PlayerPrefs.GetInt("SelectedSaveSlot", 1);
        saveLocation = Path.Combine(Application.persistentDataPath, $"saveData_{currentSlot}.json");

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
            hasCheckpoint = PlayerDataManager.instance.hasCheckpoint,
            hasBossKey = PlayerDataManager.instance.hasBossKey,
            isBossDefeated = PlayerDataManager.instance.isBossDefeated,
            hasTriggeredBossIntro = PlayerDataManager.instance.hasTriggeredBossIntro
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"Game saved in scene: {saveData.currentSceneName}");
    }

    private IEnumerator LoadGameRoutine()
    {
        yield return null;

        if (PlayerDataManager.instance == null || !PlayerDataManager.instance.shouldLoadSavedGame)
        {
            yield break;
        }

        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                if (PlayerDataManager.instance.isFastTraveling)
                {
                    PlayerDataManager.instance.isFastTraveling = false;
                }
                else if (PlayerDataManager.instance.isRespawning)
                {
                    PlayerDataManager.instance.isRespawning = false;
                }
                else if (!PlayerDataManager.instance.hasSavedData)
                {
                    player.transform.position = saveData.playerPosition;
                }
            }

            PlayerDataManager.instance.unlockedAreas = saveData.unlockedAreas;
            PlayerDataManager.instance.respawnSceneName = saveData.respawnSceneName;
            PlayerDataManager.instance.respawnPosition = saveData.respawnPosition;
            PlayerDataManager.instance.hasCheckpoint = saveData.hasCheckpoint;
            PlayerDataManager.instance.hasBossKey = saveData.hasBossKey;
            PlayerDataManager.instance.isBossDefeated = saveData.isBossDefeated;
            PlayerDataManager.instance.hasTriggeredBossIntro = saveData.hasTriggeredBossIntro;

            if (UIManager.instance != null)
            {
                UIManager.instance.RefreshMapButtons();
            }
        }

        PlayerDataManager.instance.shouldLoadSavedGame = false;
    }
}
