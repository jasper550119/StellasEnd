using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;
    
    [Header("近戰連擊設定")]
    public float meleeCooldown = 2f;      // 近戰專屬冷卻時間
    private float nextMeleeTime = 0f;     // 記錄下次可以近戰的時間點

    [Header("衝刺攻擊設定")]
    public float dashRange = 6f; 
    public float dashCooldown = 4f;       // 衝刺專屬冷卻時間 (通常大招會設長一點)
    private float nextDashTime = 0f;      // 記錄下次可以衝刺的時間點

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();

        boss.isAttacking = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();
        float distance = Vector2.Distance(player.position, rb.position);

        // 1. 判斷近戰攻擊：距離夠近，且「近戰冷卻」已完畢
        if (distance <= attackRange && Time.time >= nextMeleeTime)
        {
            int combo = Random.Range(1, 4); 
            animator.SetInteger("PendingAttacks", combo - 1); 
            animator.SetTrigger("Attack");
            
            // 更新下次近戰時間
            nextMeleeTime = Time.time + meleeCooldown + (combo * 0.5f); 
        }
        // 2. 判斷衝刺攻擊：距離夠遠，且「衝刺冷卻」已完畢
        else if (distance >= dashRange && Time.time >= nextDashTime)
        {
            animator.SetTrigger("DashAttack");
            
            // 更新下次衝刺時間
            nextDashTime = Time.time + dashCooldown; 
        }
        // 3. 移動邏輯：只要玩家不在近戰攻擊範圍內，就走向玩家
        else if (distance > attackRange)
        {
            // 【優化】：即使玩家距離大於 dashRange，但如果「衝刺還在冷卻中」，
            // Boss 就不會發呆，而是會乖乖走路逼近玩家。
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 離開追擊狀態時，清除 Trigger 避免動畫錯亂
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("DashAttack");
    }
}