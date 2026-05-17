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
    
    public bool isInvincible { get; private set; }
    private PlayerMovement playerMovement;

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
        playerMovement = GetComponent<PlayerMovement>();

        if (PlayerDataManager.instance != null)
        {
            if (PlayerDataManager.instance.isRespawning)
            {
                // 情況 A：死亡重生
                transform.position = PlayerDataManager.instance.respawnPosition;
                MaxHp();
            }
            else if (PlayerDataManager.instance.isFastTraveling)
            {
                // 情況 B：快速傳送
                transform.position = PlayerDataManager.instance.fastTravelPosition;
                currentHealth = PlayerDataManager.instance.savedHealth;
                currentPotion = PlayerDataManager.instance.savedPotions;
            }
            else if (PlayerDataManager.instance.isTransitioning) 
            {
                // 【新增】情況 D：走過傳送門
                currentHealth = PlayerDataManager.instance.savedHealth;
                currentPotion = PlayerDataManager.instance.savedPotions;

                // 尋找場景中所有的 SpawnPoint，比對 ID
                SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
                foreach (SpawnPoint sp in spawnPoints)
                {
                    if (sp.spawnID == PlayerDataManager.instance.targetSpawnID)
                    {
                        transform.position = sp.transform.position; // 移動玩家到生成點
                        break;
                    }
                }

                // 座標設定完畢，清除標記防呆
                PlayerDataManager.instance.isTransitioning = false; 
            }
            else if (PlayerDataManager.instance.hasSavedData)
            {
                // 情況 C：無指定目標的過場 (保留做為備用防呆)
                currentHealth = PlayerDataManager.instance.savedHealth;
                currentPotion = PlayerDataManager.instance.savedPotions;
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (isInvincible || (playerMovement != null && playerMovement.isDashing))
        {
            return; 
        }

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

                // 啟動協程，稍微等待死亡動畫播放後再跳出結算畫面
                StartCoroutine(DeathRoutine());
            }
        }
    }

    private IEnumerator DeathRoutine()
    {
        // 這裡的秒數可以依照你的死亡動畫長度來調整
        yield return new WaitForSeconds(1.5f); 
        
        // 呼叫 UIManager 顯示 Game Over 面板
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowGameOver();
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

        if (PlayerDataManager.instance != null && PlayerDataManager.instance.hasCheckpoint)
        {
            string currentScene = SceneManager.GetActiveScene().name;
        
            if (currentScene != PlayerDataManager.instance.respawnSceneName)
            {
                PlayerDataManager.instance.isRespawning = true;
                SceneManager.LoadScene(PlayerDataManager.instance.respawnSceneName);
                return; 
            }
            else
            {
                transform.position = PlayerDataManager.instance.respawnPosition;
            }
        }
        else if (checkpoint != null) 
        {
            transform.position = checkpoint.position; 
        }

        MaxHp();
        anim.SetBool("isDead", false);
        anim.Play("Idle");
        StartCoroutine(Invunerability());
        GetComponent<PlayerMovement>().enabled = true;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    private IEnumerator Invunerability()
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(8, 9, true);

        for (int i = 0; i < FlashTimes; i++)
        {
            spriterend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
            spriterend.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
        }

        Physics2D.IgnoreLayerCollision(8, 9, false);
        isInvincible = false;
    }
}