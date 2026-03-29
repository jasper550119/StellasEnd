using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;

    [SerializeField] private Behaviour[] components;

    // === 新增：閃爍效果變數 ===
    [Header("Flash Effect")]
    [SerializeField] private float flashDuration = 0.2f; // 閃爍總時間
    [SerializeField] private int flashTimes = 2;         // 閃爍次數
    private SpriteRenderer spriterend;                   // 控制圖片顏色的組件
    // =========================

    void Start()
    {
        anim = GetComponent<Animator>();
        spriterend = GetComponent<SpriteRenderer>();     // 取得 SpriteRenderer
        currentHealth = health;
    }

    void Update()
    {
        // 偵測血量減少 (受傷)
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
            
            // === 新增：觸發閃紅光協程 ===
            if (gameObject.activeInHierarchy) // 確保物件還活著才啟動協程
            {
                StartCoroutine(FlashRed());
            }
            // =========================
        }

        // 偵測死亡
        if(health <= 0)
        {
            anim.SetTrigger("Dead");
            Debug.Log("Enemy is dead");

            foreach (Behaviour component in components)
            {
                component.enabled = false;
            }
            
            // 死亡時關閉自己的腳本，避免重複觸發 Update
            this.enabled = false; 
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // === 新增：閃紅光視覺效果協程 ===
    private IEnumerator FlashRed()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            // 變成紅色並帶有半透明 (數值與玩家腳本一致)
            spriterend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(flashDuration / (flashTimes * 2));
            
            // 恢復原本的白色
            spriterend.color = Color.white;
            yield return new WaitForSeconds(flashDuration / (flashTimes * 2));
        }
    }
}  