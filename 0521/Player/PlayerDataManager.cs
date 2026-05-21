using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    // --- 跨場景攜帶的資料 ---
    public float savedHealth;
    public float savedPotions;
    public float savedClips;
    public bool hasSavedData = false;

    public bool hasBossKey = false;
    public bool isBossDefeated = false;

    // --- 跨場景重生的資料 ---
    public string respawnSceneName;   
    public Vector2 respawnPosition;   
    public bool hasCheckpoint = false;
    public bool isRespawning = false; 
    public bool[] unlockedAreas;

    // --- 快速傳送專用的資料 ---
    public bool isFastTraveling = false; 
    public Vector2 fastTravelPosition; 

    // 【新增】跨場景傳送門專用的資料
    public bool isTransitioning = false;  // 標記是否剛走過傳送門
    public string targetSpawnID;          // 記錄玩家要去的目標生成點 ID

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerData(float health, float potions, float clips)
    {
        savedHealth = health;
        savedPotions = potions;
        savedClips = clips;
        hasSavedData = true;
    }

    public void SetCheckpoint(string sceneName, Vector2 position)
    {
        respawnSceneName = sceneName;
        respawnPosition = position;
        hasCheckpoint = true;
    }

    public void ResetForNewGame()
    {
        savedHealth = 0f;
        savedPotions = 0f;
        savedClips = 0f;
        hasSavedData = false;

        hasBossKey = false;
        isBossDefeated = false;

        respawnSceneName = string.Empty;
        respawnPosition = Vector2.zero;
        hasCheckpoint = false;
        isRespawning = false;
        unlockedAreas = null;

        isFastTraveling = false;
        fastTravelPosition = Vector2.zero;

        isTransitioning = false;
        targetSpawnID = string.Empty;
    }
}
