using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Fast Travel System & Map")]
    [SerializeField] private GameObject worldMapPanel;       // 世界地圖/縮圖面板介面
    [SerializeField] private Button[] mapThumbnailButtons;   // 各區域的縮圖按鈕
    [SerializeField] private FastTravelPoint[] fastTravelPoints; // 存放各區域營火的場景與座標
    
    [Header("UI Objects")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject playerStats;
    [SerializeField] private GameObject mapPage;
    [SerializeField] private GameObject volumeSetting;

    [Header("Player References")]
    [SerializeField] private GameObject playerObject; 

    private PlayerMovement moveScript;
    private PlayerAttack attackScript;
    private PlayerShoot shootScript;
    private bool isPaused;
    private bool isMaped;

    [System.Serializable]
    public class FastTravelPoint
    {
        public string areaName;         // 僅供在 Inspector 中辨識用
        public string sceneName;        // 該營火所在的「場景名稱」
        public Vector2 bonfirePosition; // 該營火的 X, Y 座標
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (playerObject != null)
        {
            moveScript = playerObject.GetComponent<PlayerMovement>();
            attackScript = playerObject.GetComponent<PlayerAttack>();
            shootScript = playerObject.GetComponent<PlayerShoot>();
        }
        else
        {
            Debug.LogWarning("UIManager: 場景中找不到 Player 物件，部分功能可能無法運作。");
        }

        isPaused = false;
        isMaped = false;
        CloseAllScreens();
        if (playerStats != null) playerStats.SetActive(true);
        Time.timeScale = 1;

        if (mapThumbnailButtons != null && mapThumbnailButtons.Length > 0)
        {
            if (PlayerDataManager.instance != null)
            {
                if (PlayerDataManager.instance.unlockedAreas == null || 
                    PlayerDataManager.instance.unlockedAreas.Length != mapThumbnailButtons.Length)
                {
                    PlayerDataManager.instance.unlockedAreas = new bool[mapThumbnailButtons.Length];
                }

                for (int i = 0; i < mapThumbnailButtons.Length; i++)
                {
                    if (mapThumbnailButtons[i] != null)
                    {
                        mapThumbnailButtons[i].interactable = PlayerDataManager.instance.unlockedAreas[i];
                    }
                    else
                    {                   
                        Debug.LogWarning("注意：UIManager 的 mapThumbnailButtons 陣列第 " + i + " 個欄位是空的！請檢查 Inspector 設定。");
                    }
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (Input.GetButtonDown("Map") && !isPaused)
        {
            if (isMaped)
                ResumeGame();
            else   
                OpenMap();
        }
    }

    public void UnlockMapArea(int mapIndex)
    {
        if (PlayerDataManager.instance != null && mapIndex >= 0 && mapIndex < mapThumbnailButtons.Length)
        {
            PlayerDataManager.instance.unlockedAreas[mapIndex] = true;                 
            mapThumbnailButtons[mapIndex].interactable = true; 
            Debug.Log("已解鎖地圖區域索引：" + mapIndex);
        }
    }

    public void FastTravel(int mapIndex)
    {
        Debug.Log("嘗試傳送至索引: " + mapIndex);
    
        if (PlayerDataManager.instance != null && 
            mapIndex >= 0 && 
            mapIndex < fastTravelPoints.Length && 
            PlayerDataManager.instance.unlockedAreas[mapIndex]) 
        {
            FastTravelPoint target = fastTravelPoints[mapIndex];

            // 傳送前先紀錄玩家當前狀態
            if (playerObject != null)
            {
                PlayerHP hpScript = playerObject.GetComponent<PlayerHP>();
                PlayerShoot shootScript = playerObject.GetComponent<PlayerShoot>();
                if (hpScript != null && shootScript != null)
                {
                    PlayerDataManager.instance.SavePlayerData(hpScript.currentHealth, hpScript.currentPotion, shootScript.currentClips);
                }
            }

            // 寫入傳送專用座標
            PlayerDataManager.instance.fastTravelPosition = target.bonfirePosition;
            PlayerDataManager.instance.isFastTraveling = true; 

            Time.timeScale = 1; 
            isMaped = false;
        
            if (worldMapPanel != null) worldMapPanel.SetActive(false);
            if (mapPage != null) mapPage.SetActive(false);

            SceneManager.LoadScene(target.sceneName);
        }
        else
        {
            Debug.LogWarning("傳送失敗！可能原因：區域尚未解鎖、索引錯誤，或是 PlayerDataManager 不存在。");
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        
        if (pauseScreen != null) pauseScreen.SetActive(true);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);

        if (moveScript != null) moveScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
        if (shootScript != null) shootScript.enabled = false;
    }

    public void OpenMap()
    {
        isMaped = true;
        
        if (mapPage != null) mapPage.SetActive(true);
        if (worldMapPanel != null) worldMapPanel.SetActive(true);

        if (moveScript != null) moveScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
        if (shootScript != null) shootScript.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        isMaped = false;

        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (playerStats != null) playerStats.SetActive(true);
        if (mapPage != null) mapPage.SetActive(false);
        if (worldMapPanel != null) worldMapPanel.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);

        if (moveScript != null) moveScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
        if (shootScript != null) shootScript.enabled = true;
    }

    private void CloseAllScreens()
    {
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (worldMapPanel != null) worldMapPanel.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);
    }

    public void Vsetting()
    {
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(true);
    }

    public void BackPause()
    {
        if (pauseScreen != null) pauseScreen.SetActive(true);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
