using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;

    [SerializeField] private Behaviour[] components;

    [Header("Flash Effect")]
    [SerializeField] private float flashDuration = 0.2f; 
    [SerializeField] private int flashTimes = 2;         
    private SpriteRenderer spriterend;                   

    void Start()
    {
        anim = GetComponent<Animator>();
        spriterend = GetComponent<SpriteRenderer>();     
        currentHealth = health;
    }

    void Update()
    {
        // 偵測血量減少 (受傷)
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
            
            // === 新增：被攻擊時自動回頭轉向玩家 ===
            GameObject player = GameObject.FindWithTag("Player");
            EnemyPatrol patrol = GetComponentInParent<EnemyPatrol>();
            
            if (player != null && patrol != null)
            {
                patrol.FacePlayer(player.transform.position);
            }
            // ===================================
            
            if (gameObject.activeInHierarchy) 
            {
                StartCoroutine(FlashRed());
            }
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
            
            this.enabled = false; 
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator FlashRed()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            spriterend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(flashDuration / (flashTimes * 2));
            
            spriterend.color = Color.white;
            yield return new WaitForSeconds(flashDuration / (flashTimes * 2));
        }
    }
}