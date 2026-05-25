using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Attack : StateMachineBehaviour
{
    Boss boss;
    Rigidbody2D rb;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 取得必要組件
        if (boss == null) boss = animator.GetComponent<Boss>();
        if (rb == null) rb = animator.GetComponent<Rigidbody2D>();

        // 💡 1. 鎖定面向 (Boss 不會再跟著玩家轉向)
        boss.isAttacking = true;

        // 💡 2. 徹底停止移動 (清除任何滑步慣性)
        rb.velocity = Vector2.zero;

        // --- 原本的連擊次數扣減邏輯 ---
        int pending = animator.GetInteger("PendingAttacks");
        if (pending > 0)
        {
            animator.SetInteger("PendingAttacks", pending - 1);
        }
    }
}