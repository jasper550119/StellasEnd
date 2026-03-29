using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public bool isFlipped = false;
    
    // 💡 新增：用來記錄 Boss 是否正在攻擊
    public bool isAttacking = false; 

    public void LookAtPlayer()
    {
        // 💡 新增：如果正在攻擊，就停止轉向（維持第一下的方向）
        if (isAttacking) 
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
}