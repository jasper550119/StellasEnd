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
    public Animator anim; 

    [Header("掉落物")]
    public GameObject keyPrefab;

    // 💡【新增】戰後對話的 NPC 物件
    [Header("戰後對話設定")]
    public NPC endDialogueNPC;

    private IEnumerator Start()
    {
        yield return null;
        
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
        if (health < 0) health = 0; 

        if (health == 0)
        {
            Die();
        }
    }

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    void Die()
    {
        isInvulnerable = true; // 💡 確保死亡動畫與對話期間 Boss 不會再受到傷害
        
        // 💡 1. 照常觸發死亡動畫 (如倒地或喘氣)
        if (anim != null) anim.SetTrigger("Die");
        if (healthBarUI != null) healthBarUI.SetActive(false);

        // 💡 2. 判斷是否有戰後對話
        if (endDialogueNPC != null)
        {
            endDialogueNPC.StartDialogue(); // 觸發戰後對話 (此時玩家會被 NPC.cs 自動鎖定無法移動)
        }
        else
        {
            // 防呆：如果忘記掛戰後對話，就直接死
            ExecuteDeathFeatures();
        }

        // 停用此 HP 腳本，防止重複觸發 Die()
        this.enabled = false;
    }

    // 💡【新增】專門處理「除動畫以外」的死亡相關功能
    // 這個方法會在戰後對話結束後，透過 UnityEvent 被呼叫
    public void ExecuteDeathFeatures()
    {
        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.isBossDefeated = true;
            PlayerDataManager.instance.hasTriggeredBossIntro = true;
        }

        SaveController saveController = Object.FindFirstObjectByType<SaveController>();
        if (saveController != null)
        {
            saveController.SaveGame();
        }

        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, transform.position, Quaternion.identity);
        }

        // 因為前面對話花費了時間，動畫早就播完了，這裡可以直接銷毀 Boss 物件
        Destroy(gameObject);
    }
}
