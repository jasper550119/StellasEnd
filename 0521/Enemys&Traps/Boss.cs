using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public bool isFlipped = false;
    public bool isAttacking = false; 

    // 💡【新增】紀錄 Boss 是否正式進入戰鬥
    public bool isCombatStarted = false; 

    public void LookAtPlayer()
    {
        // 💡【修改】如果還沒開始戰鬥，或者正在攻擊，都不進行轉向
        if (!isCombatStarted || isAttacking) 
            return; 

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    // 💡【新增】供開場對話結束（UnityEvent）呼叫的方法
    public void StartCombat()
    {
        isCombatStarted = true;
    }
}