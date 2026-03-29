using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack2 : MonoBehaviour
{
    public float damage;
    public float radius;
    public LayerMask enemies;
    public GameObject attackPoint;
    private PlayerMovement Move;

    [Header("動畫")]
    private Animator anim;
    
    [Header("判斷")]
    private CircleCollider2D attackHitbox;
    private ContactFilter2D enemyFilter;
    private bool attacking = false; // 記錄是否正在攻擊中

    void Start()
    {
        anim = GetComponent<Animator>();
        attackHitbox = this.transform.GetChild(0).GetComponent<CircleCollider2D>();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        Move = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // 當按下攻擊鍵，且目前「沒有」在攻擊狀態時，執行攻擊
        if (Input.GetButtonDown("Attack") && !attacking)
        {
            NormalAttack();
        }
    }

    void NormalAttack()
    {
        attacking = true;
        Move.enabled = false; // 攻擊時禁止移動
        anim.SetTrigger("attack_1"); // 觸發單次攻擊動畫
    }

    // 動畫事件 (Animation Event) - 在武器揮擊到判定點時呼叫
    public void attack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach(Collider2D hitObject in hitColliders)
        {
            if (hitObject.CompareTag("Enemy"))
            {
                EnemyHP enemyHpScript = hitObject.GetComponent<EnemyHP>();
                if (enemyHpScript != null)
                {
                    enemyHpScript.health -= damage; 
                }

                BossHP bossHpScript = hitObject.GetComponent<BossHP>();
                if (bossHpScript != null)
                {
                    bossHpScript.TakeDamage((int)damage); 
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }

    // 動畫事件 (Animation Event) - 在攻擊動畫「最後一幀」呼叫
    public void BackIdle()
    {
        attacking = false;
        Move.enabled = true; // 恢復移動
    }
}