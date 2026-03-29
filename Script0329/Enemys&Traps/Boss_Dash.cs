using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Dash : StateMachineBehaviour
{
    public float dashSpeed = 10f; 
    
    Transform player;
    Rigidbody2D rb;
    Vector2 targetPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        
        targetPos = new Vector2(player.position.x, rb.position.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 檢查 Boss 和目標點的距離
        float distance = Vector2.Distance(rb.position, targetPos);

        // 只有在還沒到達目標點時（距離大於 0.1）才繼續移動
        if (distance > 0.1f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, dashSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            // 如果已經到達目標點，可以強制提早結束衝刺狀態，回到 Run 或 Idle
            // animator.SetTrigger("StopDash"); // (可選) 如果你想提早卡掉動畫可以加這行
        }
    }
}