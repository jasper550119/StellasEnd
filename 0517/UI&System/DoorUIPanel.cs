using UnityEngine;

public class DoorUIPanel : MonoBehaviour
{
    [Header("關聯目標門物件")]
    [SerializeField] private LockedDoor targetDoor;

    [Header("音效設定")]
    [SerializeField] private AudioClip successSound; // 開門成功音效
    [SerializeField] private AudioClip failureSound; // 開門失敗音效
    [SerializeField] private AudioSource audioSource; // 播放音效用的 AudioSource (可選)

    /// <summary>
    /// 綁定在 UI 按鈕上的方法
    /// </summary>
    public void OnClickUnlockButton()
    {
        if (PlayerDataManager.instance == null)
        {
            Debug.LogError("找不到 PlayerDataManager 實例！");
            return;
        }

        // 偵測玩家是否持有鑰匙
        if (PlayerDataManager.instance.hasBossKey)
        {
            // 【成功】
            PlaySound(successSound);
            
            if (targetDoor != null)
            {
                targetDoor.OpenDoor(); // 執行實體門開啟
            }
            
            ClosePanel(); // 關閉面板
        }
        else
        {
            // 【失敗】按鈕無作用，僅播放失敗音效
            PlaySound(failureSound);
            Debug.Log("玩家未持有鑰匙，解鎖失敗。");
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        // 如果有指定 AudioSource 就用它播放，沒有的話就使用靜態方法在相機位置播放
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
}