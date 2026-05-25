using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public float damage;
    public float radius;
    public LayerMask enemies;
    public GameObject attackPoint;
    private PlayerMovement Move;
    [Header("動畫")]
    private Animator anim;      //just...animator
    
    [Header("判斷")]
    private CircleCollider2D attackHitbox;  //attck range
    private ContactFilter2D enemyFilter;    //enemy YES/NO
    private int normalattack_step = 0;      //attack1 ?,2 ?, 3?
    private bool attacking = false;         //attck"ing"?, end -> next attack

    [Header("Input")]
    private float skillInputDelay = 0.5f;    //0.5sec delay input
    private int normalattack_input = 0;      //count how many attack
    private Coroutine inputDelatRoutine;    //for delay
    private Coroutine attackLockRoutine;

    void Start()
    {
        anim = GetComponent<Animator>();
        attackHitbox = this.transform.GetChild(0).GetComponent<CircleCollider2D>();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        Move = GetComponent<PlayerMovement>();
    }

    void SetAttacking(bool truefalse)
    {
        attacking = truefalse;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            normalattack_input += 1;

            if (Move != null) Move.SetControl(false);
        }

        if (normalattack_input > normalattack_step)
        {
            if (!attacking)NormalAttack();
        }
    }

    void NormalAttack()
    {
        SetAttacking(true);
        normalattack_step += 1;

        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);
        attackLockRoutine = StartCoroutine(ReleaseMovementAfterDelay(1.5f));

        switch (normalattack_step)
        {
            case 1:
                {
                    anim.SetTrigger("attack_1");

                    if (inputDelatRoutine != null) StopCoroutine(inputDelatRoutine);
                    inputDelatRoutine = StartCoroutine(Normalinput_Delaying());
                }
                break;
            case 2:
                {
                    anim.SetTrigger("attack_2");

                    if (inputDelatRoutine != null) StopCoroutine(inputDelatRoutine);
                    inputDelatRoutine = StartCoroutine(Normalinput_Delaying());
                }
                break;
            case 3:
                {
                    anim.SetTrigger("attack_3");

                    if (inputDelatRoutine != null) StopCoroutine(inputDelatRoutine);
                }
                break;

            IEnumerator Normalinput_Delaying()
            {
                float elapsed = 0f;

                while (elapsed < skillInputDelay) { elapsed += Time.deltaTime; yield return null; }

                BackIdle();
            }
        }

    }

    public void attack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach(Collider2D hitObject in hitColliders)
        {
            // 判斷 Tag 是否為 "Enemy"
            if (hitObject.CompareTag("Enemy"))
            {
                // 情況 A：嘗試獲取一般敵人的腳本
                EnemyHP enemyHpScript = hitObject.GetComponent<EnemyHP>();
                if (enemyHpScript != null)
                {
                    enemyHpScript.health -= damage; 
                }

                // 情況 B：嘗試獲取 Boss 的腳本
                BossHP bossHpScript = hitObject.GetComponent<BossHP>();
                if (bossHpScript != null)
                {
                    // 因為 BossHP 的 damage 是 int，而 PlayerAttack 的 damage 是 float，這裡做個轉型 (int)
                    bossHpScript.TakeDamage((int)damage); 
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }

    public void BackIdle()
    {
        SetAttacking(false);
        if (inputDelatRoutine != null ) StopCoroutine(inputDelatRoutine);

        normalattack_step = 0;
        normalattack_input = 0;
        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);
        if (Move != null) Move.SetControl(true);
    }

    private IEnumerator ReleaseMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (attacking || normalattack_input > 0 || normalattack_step > 0)
        {
            BackIdle();
        }
    }

    private void OnDisable()
    {
        if (inputDelatRoutine != null) StopCoroutine(inputDelatRoutine);
        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);

        attacking = false;
        normalattack_step = 0;
        normalattack_input = 0;

        if (Move != null)
        {
            Move.SetControl(true);
        }
    }

    void ThisStepFinish()
    {
        attacking = false;
        if (normalattack_step >= 3) BackIdle();
    }

}
