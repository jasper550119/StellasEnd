using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("下一個場景的名稱")]
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 確認碰到的是玩家
        if (other.CompareTag("Player"))
        {
            // 取得玩家身上的腳本
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();

            // 確保玩家身上有這些腳本且 Manager 存在
            if (playerHP != null && playerShoot != null && PlayerDataManager.instance != null)
            {
                // 將目前的數值存入 Manager 中
                PlayerDataManager.instance.SavePlayerData(playerHP.currentHealth, playerHP.currentPotion, playerShoot.currentClips);
            }

            // 載入下一個場景
            SceneManager.LoadScene(nextSceneName);
        }
    }
}