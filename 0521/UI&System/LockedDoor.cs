using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入場景管理

[RequireComponent(typeof(AudioSource))] // 自動確保物件上帶有 AudioSource 元件
public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("傳送設定")]
    public string targetSceneName;   // 要傳送過去的目標場景名稱
    public string targetSpawnID;     // 配合你的 PlayerDataManager，設定目標生成點 ID

    [Header("音效設定")]
    public AudioClip successSound;   // 成功(有鑰匙)的音效
    public AudioClip failSound;      // 失敗(沒鑰匙)的音效
    
    private AudioSource audioSource;
    private bool isLoading = false;  // 防止連按 E 鍵重複觸發

    private void Start()
    {
        // 初始化音效播放器
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public bool CanInteract()
    {
        // 只要還沒開始傳送，隨時都能互動 (這樣沒鑰匙按 E 才能播失敗音效)
        return !isLoading;
    }

    public void Interact()
    {
        if (isLoading) return; // 防呆：如果已經在載入中就直接退出

        // 檢查是否有鑰匙
        if (PlayerDataManager.instance != null && PlayerDataManager.instance.hasBossKey)
        {
            // 【成功】啟動傳送流程
            StartCoroutine(TransitionRoutine());
        }
        else
        {
            // 【失敗】播放沒鑰匙的音效，無其他效果
            PlaySound(failSound);
            Debug.Log("【系統提示】你沒有 Boss 鑰匙，門沒有反應。");
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isLoading = true;

        // 【新增修改】在切換場景前，先動態尋找玩家並儲存目前的數值（修正問題 1）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHP playerHP = player.GetComponent<PlayerHP>();
            PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

            if (playerHP != null && playerShoot != null && PlayerDataManager.instance != null)
            {
                // 將玩家目前的血量、藥水、子彈儲存至單例中，這樣新場景才能順利繼承
                PlayerDataManager.instance.SavePlayerData(playerHP.currentHealth, playerHP.currentPotion, playerShoot.currentClips);
            }
        }

        // 1. 設定跨場景資料 (完美利用你 PlayerDataManager 寫好的變數！)
        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.isTransitioning = true;
            PlayerDataManager.instance.targetSpawnID = this.targetSpawnID; // （回應問題 2）
        }

        // 2. 播放成功音效
        PlaySound(successSound);

        // 3. 延遲等待音效播完 (如果沒設定音效，預設等 0.5 秒)
        float delayTime = successSound != null ? successSound.length : 0.5f;
        yield return new WaitForSeconds(delayTime);

        // 4. 切換場景
        Debug.Log($"準備傳送至場景：{targetSceneName}，生成點 ID：{targetSpawnID}");
        SceneManager.LoadScene(targetSceneName);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}