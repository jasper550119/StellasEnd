using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private CapsuleCollider2D coll; // 確認你的 Unity Inspector 裡掛的是 CapsuleCollider2D 喔！

    private bool hit;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        if (hit) return;

        // ⚠️ 注意：這裡改成 Y 軸移動了。如果你的子彈是橫向飛的，請改回 (movementSpeed, 0, 0)
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(0, movementSpeed, 0); 

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    // EnemyDamage.cs 改良版
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
    // 1. 先用 Tag 過濾（效能好）
        if (collision.CompareTag("Player"))
            {
        // 2. 往父層找組件（解決子物件問題）
                PlayerHP player = collision.GetComponentInParent<PlayerHP>();
        
                if (player != null)
                {
                    hit = true;
                    player.TakeDamage(damage);
                    coll.enabled = false;

                    Deactivate();
                }
            }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}