using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    public int health = 50;
    public Slider healthBar;
    public GameObject healthBarUI;
    public bool isInvulnerable = false;

    [Header("動畫")]
    public Animator anim; // 記得在 Unity 面板把 Boss 的 Animator 拖曳進來

    [Header("掉落物")]
    public GameObject keyPrefab;

    private void Start()
    {
        if (PlayerDataManager.instance != null && PlayerDataManager.instance.isBossDefeated)
        {
            if (healthBarUI != null) healthBarUI.SetActive(false);
            Destroy(gameObject); 
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;

        // 確保血量條不要變成負數
        if (health < 0) health = 0; 

        if (health == 0)
        {
            Die();
        }
    }

    private void Update()
    {
        // 如果 healthBar 沒有設定，加個防呆避免報錯
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    void Die()
    {
        // 1. 播放死亡動畫 (請確保你的 Animator 裡面有一個名為 "Die" 的 Trigger 參數)
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.isBossDefeated = true;
        }

        // 4. 延遲銷毀物件：給動畫播放的時間 (例如延遲 1.5 秒，請依照你的動畫長度調整)
        Destroy(gameObject, 1.5f);

        // 關閉這個腳本，避免重複執行
        this.enabled = false;

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false); // 直接隱藏整個血條物件
        }
    }
}