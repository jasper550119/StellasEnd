using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    //References
    private Animator anim;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            // 只要冷卻時間到了，就嘗試觸發攻擊動畫
            if (cooldownTimer >= attackCooldown)
            {
                // ⚠️ 注意：不要在這裡歸零 cooldownTimer
                // 讓 cooldownTimer 繼續跑，直到動畫事件真的觸發了 RangedAttack 才歸零
                // 這樣可以避免動畫還沒播出來，計時器就被重置的問題
                anim.SetTrigger("Attack");
                
                // 為了防止一幀內觸發多次 Trigger，可以在這裡設一個保險，
                // 或是乾脆把 timer 歸零移到這裡也可以，看你動畫配合度。
                // 比較保險的做法是：
                cooldownTimer = 0; 
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    // 這個函式由 Animation Event 呼叫
    private void RangedAttack()
    {
        // 1. 確保冷卻歸零 (雙重保險，確保發射後一定要重算)
        cooldownTimer = 0; 

        // 2. 取得子彈索引
        int fireballIndex = FindFireball();
        
        // 3. 取得該子彈物件
        GameObject currentFireball = fireballs[fireballIndex];

        // 4. 設定位置與旋轉 (這一步很重要，確保子彈方向正確)
        currentFireball.transform.position = firepoint.position;
        currentFireball.transform.rotation = firepoint.rotation; // 跟隨發射點旋轉

        // 5. 啟動子彈
        currentFireball.GetComponent<EnemyProjectile>().ActivateProjectile();
    }
    
    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}