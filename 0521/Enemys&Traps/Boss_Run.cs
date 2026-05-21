using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;
    
    [Header("近戰連擊設定")]
    public float meleeCooldown = 2f;      
    private float nextMeleeTime = 0f;     

    [Header("衝刺攻擊設定")]
    public float dashRange = 6f; 
    public float dashCooldown = 4f;       
    private float nextDashTime = 0f;      

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
        // 💡【新增】如果戰鬥還沒開始，強制停在原地，不執行後續任何追擊或攻擊邏輯
        if (boss != null && !boss.isCombatStarted)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        boss.LookAtPlayer();
        float distance = Vector2.Distance(player.position, rb.position);

        if (distance <= attackRange && Time.time >= nextMeleeTime)
        {
            int combo = Random.Range(1, 4); 
            animator.SetInteger("PendingAttacks", combo - 1); 
            animator.SetTrigger("Attack");
            
            nextMeleeTime = Time.time + meleeCooldown + (combo * 0.5f); 
        }
        else if (distance >= dashRange && Time.time >= nextDashTime)
        {
            animator.SetTrigger("DashAttack");
            nextDashTime = Time.time + dashCooldown; 
        }
        else if (distance > attackRange)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("DashAttack");
    }
}