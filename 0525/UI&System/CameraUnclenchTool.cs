using UnityEngine;

public static class CameraUnclenchTool
{
    /// <summary>
    /// 強制解除 Timeline 對攝影機的鎖定，並重置座標到玩家位置
    /// </summary>
    public static void ForceUnclench(CamMovement cam, Transform player)
    {
        if (cam == null || player == null)
        {
            Debug.LogWarning("Unclench Tool: 找不到攝影機或玩家，無法解除鎖定。");
            return;
        }

        // 1. 強制重啟腳本狀態
        cam.enabled = false;
        cam.isFollowing = true;
        cam.enabled = true;

        // 2. 徹底打破物理/動畫座標鎖定
        // 直接賦值給 transform.position 是打破 Animation Track 鎖定的最直接手段
        Vector3 targetPos = player.position + cam.offset;
        targetPos.z = cam.transform.position.z; // 保留 2D 深度
        
        cam.transform.position = targetPos;

        // 3. (進階) 某些情況下 Animator 會持續鎖定，強制停止物件上的 Animator
        if (cam.TryGetComponent<Animator>(out Animator anim))
        {
            anim.enabled = false;
            // 下一幀再開啟，或者維持關閉（視你的專案需求而定）
        }

        Debug.Log($"<color=cyan>攝影機鎖定已強制解除！目前座標已校準至: {targetPos}</color>");
    }
}