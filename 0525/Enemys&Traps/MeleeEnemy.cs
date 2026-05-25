using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    private float CooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Rigidbody2D rb;

    private PlayerHP playerHealth;
    private EnemyPatrol enemypatrol;

    [Header("Attack Lunge Settings")]
    [SerializeField] private bool hasLungeEffect = false;
    [SerializeField] private float lungeSpeed = 5f;
    [SerializeField] private float lungeDuration = 0.2f;
    private bool isLunging = false;

    [Header("Jump Attack Settings")]
    [SerializeField] private bool hasJumpAttack = false;
    [SerializeField] private float jumpRange = 5f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float jumpDuration = 0.3f;
    [SerializeField] private float jumpCooldown = 3f;
    private float jumpCooldownTimer = Mathf.Infinity;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // 獲取剛體以處理物理位移
        enemypatrol = GetComponentInParent<EnemyPatrol>();
    }

    void Update()
    {
        // 如果正在衝刺位移中，不執行後續邏輯，避免重複觸發
        if (isLunging) return;

        CooldownTimer += Time.deltaTime;
        jumpCooldownTimer += Time.deltaTime;

        // 1. 優先判斷普通近戰攻擊
        if (PlayerInSight(range))
        {
            if (CooldownTimer >= attackCooldown)
            {
                CooldownTimer = 0;
                anim.SetTrigger("Attack");

                if (hasLungeEffect)
                {
                    StartCoroutine(AttackLunge(lungeSpeed, lungeDuration));
                }
            }
        }
        // 2. 若不在近戰範圍，則判斷是否執行跳躍攻擊
        else if (hasJumpAttack && PlayerInSight(jumpRange))
        {
            if (jumpCooldownTimer >= jumpCooldown)
            {
                jumpCooldownTimer = 0;
                anim.SetTrigger("JumpAttack");
                StartCoroutine(AttackLunge(jumpSpeed, jumpDuration));
            }
        }

        // 巡邏控制：根據最大範圍決定是否停止巡邏
        if (enemypatrol != null)
        {
            float maxDetectionRange = (hasJumpAttack && jumpRange > range) ? jumpRange : range;
            enemypatrol.enabled = !PlayerInSight(maxDetectionRange);
        }
    }

    // 修改後的偵測邏輯：會排除 isTrigger 的碰撞體
    private bool PlayerInSight(float currentRange)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            boxCollider.bounds.center + transform.right * currentRange * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * currentRange, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        foreach (RaycastHit2D hit in hits)
        {
            // 只有當碰撞體不是 Trigger 時才觸發
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                playerHealth = hit.transform.GetComponent<PlayerHP>();
                return true; 
            }
        }

        return false; 
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));

        if (hasJumpAttack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * jumpRange * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * jumpRange, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }
    }

    private void DamagePlayer()
    {
        // 攻擊判定也需排除 trigger
        if (PlayerInSight(range))
            playerHealth.TakeDamage(damage);
    }

    // 修改後的位移邏輯：使用 Rigidbody2D 確保物理位移有效
    private IEnumerator AttackLunge(float speed, float duration)
    {
        isLunging = true;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            if (rb != null)
            {
                // 設定水平速度，同時保留垂直方向的重力影響
                rb.velocity = new Vector2(transform.localScale.x * speed, rb.velocity.y);
            }
            else
            {
                transform.Translate(Vector3.right * transform.localScale.x * speed * Time.deltaTime);
            }
            yield return null; 
        }

        // 結束後停止水平移動
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        isLunging = false;
    }
}