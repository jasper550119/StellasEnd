using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrame")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] private int FlashTimes;
    private SpriteRenderer spriterend;

    private Transform checkpoint;

    private float startingPotion = 3f;
    public float currentPotion { get; private set; }
    public float Heal;

    public void Awake()
    {
        currentHealth = startingHealth;

        checkpoint = transform;

        currentPotion = startingPotion;
    }
    public void MaxHp()
    {
        currentHealth = startingHealth;

        currentPotion = startingPotion;
    } 
    
    void Start()
    {
        anim = GetComponent<Animator>();
        spriterend = GetComponent<SpriteRenderer>();

        // 【修改】：啟動時檢查是要「正常過圖」還是「死掉復活」
        if (PlayerDataManager.instance != null)
        {
            if (PlayerDataManager.instance.isRespawning)
            {
                // 如果是跨場景復活回來的，位置設回營火，並且滿血滿狀態
                transform.position = PlayerDataManager.instance.respawnPosition;
                currentHealth = startingHealth;
                currentPotion = startingPotion;
            
                // 狀態恢復完畢，關閉復活開關
                PlayerDataManager.instance.isRespawning = false; 
            }
            else if (PlayerDataManager.instance.hasSavedData)
            {
                // 如果只是正常穿過傳送門，就讀取殘存的血量
                currentHealth = PlayerDataManager.instance.savedHealth;
                currentPotion = PlayerDataManager.instance.savedPotions;
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        
        if (currentHealth > 0)
        {
            anim.SetTrigger("Attacked");
            StartCoroutine(Invunerability());
        }
        else
        {
            if(!dead)
            {
            anim.SetBool("isDead", true);
            GetComponent<PlayerMovement>().enabled = false;
            dead = true;
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Heal"))
        {
            if(currentPotion > 0)
            {
                AddHealth(Heal);
            
                currentPotion--;
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    
    public void Respawn()
    {
        dead = false;

        // 【新增】：跨場景復活邏輯
        if (PlayerDataManager.instance != null && PlayerDataManager.instance.hasCheckpoint)
        {
            string currentScene = SceneManager.GetActiveScene().name;
        
            if (currentScene != PlayerDataManager.instance.respawnSceneName)
            {
                // 發現死掉的場景跟營火場景不同！準備跨場景讀取
                PlayerDataManager.instance.isRespawning = true;
                SceneManager.LoadScene(PlayerDataManager.instance.respawnSceneName);
                return; // ★這裡很重要！直接 Return 終止程式，剩下的交給載入場景後的 Start() 去處理
            }
            else
            {
                // 如果是同一個場景，直接把玩家瞬移過去
                transform.position = PlayerDataManager.instance.respawnPosition;
            }
        }
        else if (checkpoint != null) 
        {
            // 備用方案：如果根本沒存過檔
            transform.position = checkpoint.position; 
        }

        // 恢復狀態
        AddHealth(startingHealth);
        anim.SetBool("isDead", false);
        anim.Play("Idle");
        StartCoroutine(Invunerability());
        GetComponent<PlayerMovement>().enabled = true;
        currentPotion = startingPotion;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < FlashTimes; i++)
        {
            spriterend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
            spriterend.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }
}
