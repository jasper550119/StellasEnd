using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private TrailRenderer tr;
    public float speed = 20f;
    public Rigidbody2D rb;
    public int bulletDamage = 3;
    public float bulletLifeTime = 5f;

    void Start()
    {
        rb.velocity = transform.right * speed;
        Destroy(gameObject, bulletLifeTime);
        tr.emitting = true;
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        EnemyHP enemy = hitInfo.GetComponent<EnemyHP>();
        if(enemy != null)
        {
            enemy.health -= bulletDamage;
            Destroy(gameObject);
            tr.emitting = false;
        }
    }
}
