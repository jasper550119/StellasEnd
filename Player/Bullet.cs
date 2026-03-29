using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private TrailRenderer tr;
    public float speed = 20f;
    public Rigidbody2D rb;
    public int bulletDamage = 3;
    public float bulletLifeTime = 5f;

    void Start()
    {
        rb.velocity = transform.right * speed;
        Destroy(gameObject, bulletLifeTime);
        tr.emitting = true;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. 檢查碰撞到的物件 Tag 是否為 "Enemy"
        if (hitInfo.CompareTag("Enemy"))
        {
            // 情況 A：如果是一般敵人 (EnemyHP)
            EnemyHP enemy = hitInfo.GetComponent<EnemyHP>();
            if (enemy != null)
            {
                enemy.health -= bulletDamage;
            }

            // 情況 B：如果是 Boss (BossHP)
            BossHP boss = hitInfo.GetComponent<BossHP>();
            if (boss != null)
            {
                // 使用 BossHP 腳本內建的 TakeDamage 方法，這會處理無敵和死亡邏輯
                boss.TakeDamage(bulletDamage);
            }

            // 子彈打到敵人後消失的處理
            HandleBulletDestroy();
        }
        // 如果你希望子彈打到牆壁也會消失，可以解開下面這幾行的註解：
        /*
        else if (hitInfo.CompareTag("Ground")) 
        {
            HandleBulletDestroy();
        }
        */
    }

    // 統一處理子彈消失，避免重複寫程式碼
    void HandleBulletDestroy()
    {
        tr.emitting = false;
        // 為了讓拖尾 (Trail) 播完，可以先關閉碰撞和渲染，延遲一下下再刪除
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.1f); 
    }
}