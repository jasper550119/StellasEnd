using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    // --- 跨場景攜帶的資料 ---
    public float savedHealth;
    public float savedPotions;
    public float savedClips;
    public bool hasSavedData = false;

    // --- 跨場景重生的資料 (新增) ---
    public string respawnSceneName;   // 記錄營火所在的場景名稱
    public Vector2 respawnPosition;   // 記錄營火的 X, Y 座標
    public bool hasCheckpoint = false;// 判斷是否已經有存過重生點
    public bool isRespawning = false; // 告訴下個場景「我是死掉復活的，不要讀取門口的存檔，讓我滿血回營火」

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

    // 過場門專用的存檔
    public void SavePlayerData(float health, float potions, float clips)
    {
        savedHealth = health;
        savedPotions = potions;
        savedClips = clips;
        hasSavedData = true;
    }

    // 營火/檢查點專用的存檔 (新增)
    public void SetCheckpoint(string sceneName, Vector2 position)
    {
        respawnSceneName = sceneName;
        respawnPosition = position;
        hasCheckpoint = true;
    }
}