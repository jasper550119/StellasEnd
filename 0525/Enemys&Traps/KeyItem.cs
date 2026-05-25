using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查碰到的是否為玩家
        if (other.CompareTag("Player"))
        {
            // 1. 紀錄玩家獲得鑰匙
            if (PlayerDataManager.instance != null)
            {
                PlayerDataManager.instance.hasBossKey = true;
                
                // 【新增】在 Console 列印撿起鑰匙的文字
                Debug.Log("<color=yellow>【系統提示】玩家已成功撿起 Boss 鑰匙！</color>");
            }

            // 2. 自動存檔
            SaveController saveCtrl = Object.FindFirstObjectByType<SaveController>();
            if (saveCtrl != null)
            {
                saveCtrl.SaveGame();
            }

            // 3. 銷毀鑰匙物件
            Destroy(gameObject);
        }
    }
}