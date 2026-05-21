using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("下一個場景的名稱")]
    public string nextSceneName;

    // 【新增】讓你在 Inspector 設定這扇門要通往哪一個生成點 ID
    [Header("目標生成點 ID")]
    public string targetSpawnID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();

            if (playerHP != null && playerShoot != null && PlayerDataManager.instance != null)
            {
                // 儲存玩家數值
                PlayerDataManager.instance.SavePlayerData(playerHP.currentHealth, playerHP.currentPotion, playerShoot.currentClips);
                
                // 【新增】設定跨場景標記與目標點
                PlayerDataManager.instance.isTransitioning = true;
                PlayerDataManager.instance.targetSpawnID = targetSpawnID;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}