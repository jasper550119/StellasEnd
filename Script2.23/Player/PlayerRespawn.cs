using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 【新增】引入場景管理

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip cheackpointSound;
    private PlayerHP playerHealth;
    private PlayerShoot Gun;

    void Start()
    {
        playerHealth = GetComponent<PlayerHP>();
        Gun = GetComponent<PlayerShoot>();
    }

    public void Respawn()
    {
        // 【修改】：把 transform.position 移到 PlayerHP 統一處理
        playerHealth.Respawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Cheackpoint")
        {
            // 【修改這裡】：改存場景名稱與座標
            if (PlayerDataManager.instance != null)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                PlayerDataManager.instance.SetCheckpoint(currentScene, collision.transform.position);
            }
            
            //SoundManager.instance.PlaySound(cheackpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            Gun.Reload();
            Debug.Log("1");
        }
    }
}