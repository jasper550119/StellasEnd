using UnityEngine;
using UnityEngine.Playables;

public class AutoPlayCutscene : MonoBehaviour
{
    [Header("過場動畫設定")]
    public PlayableDirector cutsceneDirector;
    
    // 給這個動畫取個專屬名字，例如 "Scene2_Intro"
    [Header("動畫專屬ID (用來記錄是否播過)")]
    public string cutsceneID = "Scene2_Intro"; 

    [Header("玩家腳本 (用來鎖定移動)")]
    public PlayerMovement player;

    // ⭐ 新增：綁定攝影機腳本，確保播完動畫後能要回控制權
    [Header("攝影機設定")]
    public CamMovement mainCamera;

    private void Start()
    {
        // 1. 檢查這個 ID 是否有被記錄為 1 (代表播過)
        if (PlayerPrefs.GetInt(cutsceneID, 0) == 1)
        {
            // 如果已經播過，就把動畫物件直接關閉，然後什麼都不做
            if (cutsceneDirector != null)
                cutsceneDirector.enabled = false;

            if (player != null) player.canControl = true;
            
            return; 
        }   
    
        // 2. 如果沒播過，準備播放動畫
        if (cutsceneDirector != null)
        {
            // 鎖住玩家操作
            if (player != null) 
                player.canControl = false;
            
            // 訂閱 Timeline 的「停止事件」(當動畫播完時，會自動呼叫 OnCutsceneEnded 方法)
            cutsceneDirector.stopped += OnCutsceneEnded; 
            
            // 開始播放動畫
            cutsceneDirector.Play();
        }
    }

    // 當 Timeline 徹底播完時，會觸發這個方法
    private void OnCutsceneEnded(PlayableDirector director)
    {
        // 記錄下來：這個動畫已經播過了！(存入 1)
        PlayerPrefs.SetInt(cutsceneID, 1);
        PlayerPrefs.Save(); // 確保存檔

        // 解除玩家操作鎖定，讓玩家可以移動
        if (player != null) 
            player.canControl = true;

        // ⭐ 新增：強制奪回攝影機控制權並瞬間歸位，防止卡死或異常拉扯
        if (mainCamera != null && player != null)
        {
            mainCamera.enabled = true;
            mainCamera.isFollowing = true;
            
            // 計算理想位置並強制賦值 (保留原本的 Z 軸，避免 2D 畫面消失)
            Vector3 resetPos = player.transform.position + mainCamera.offset;
            resetPos.z = mainCamera.transform.position.z;
            mainCamera.transform.position = resetPos;
        }

        // 取消訂閱事件 (好習慣，避免報錯)
        cutsceneDirector.stopped -= OnCutsceneEnded;
    }

    // 💡 測試用小功能：如果你在編輯器裡想重製播放紀錄，可以按右鍵選這個
    [ContextMenu("重置播放紀錄 (Reset Cutscene)")]
    public void ResetCutsceneRecord()
    {
        PlayerPrefs.SetInt(cutsceneID, 0);
        PlayerPrefs.Save();
        Debug.Log("已重置動畫播放紀錄！下次進入場景會再次播放。");
    }
}