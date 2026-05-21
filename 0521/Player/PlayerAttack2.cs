using System.Collections;
using UnityEngine;

public class PlayerAttack2 : MonoBehaviour
{
    public float damage;
    public float radius;
    public LayerMask enemies;
    public GameObject attackPoint;

    private PlayerMovement Move;
    private Animator anim;
    private CircleCollider2D attackHitbox;
    private ContactFilter2D enemyFilter;
    private bool attacking = false;
    private Coroutine attackLockRoutine;

    void Start()
    {
        anim = GetComponent<Animator>();
        attackHitbox = transform.GetChild(0).GetComponent<CircleCollider2D>();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        Move = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Attack") && !attacking)
        {
            NormalAttack();
        }
    }

    void NormalAttack()
    {
        attacking = true;
        if (Move != null) Move.SetControl(false);

        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);
        attackLockRoutine = StartCoroutine(ReleaseMovementAfterDelay(1.2f));

        anim.SetTrigger("attack_1");
    }

    public void attack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D hitObject in hitColliders)
        {
            if (!hitObject.CompareTag("Enemy")) continue;

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

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }

    public void BackIdle()
    {
        attacking = false;

        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);
        if (Move != null) Move.SetControl(true);
    }

    private IEnumerator ReleaseMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (attacking)
        {
            BackIdle();
        }
    }

    private void OnDisable()
    {
        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);

        attacking = false;

        if (Move != null)
        {
            Move.SetControl(true);
        }
    }
}
