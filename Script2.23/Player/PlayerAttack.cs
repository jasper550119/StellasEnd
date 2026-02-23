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

            Move.enabled = false;
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
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach(Collider2D enemyGameobject in enemy)
        {
            Debug.Log("Hit enemy");
            enemyGameobject.GetComponent<EnemyHP>().health -= damage;
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
        Move.enabled = true;
    }

    void ThisStepFinish()
    {
        attacking = false;
        if (normalattack_step >= 3) BackIdle();
    }

}
