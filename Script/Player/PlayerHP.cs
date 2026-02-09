using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrame")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] private int FlashTimes;
    private SpriteRenderer spriterend;

    private Transform checkpoint;

    private float startingPotion = 3f;
    public float currentPotion { get; private set; }
    public float Heal;

    public void Awake()
    {
        currentHealth = startingHealth;

        checkpoint = transform;

        currentPotion = startingPotion;
    }
    public void MaxHp()
    {
        currentHealth = startingHealth;

        currentPotion = startingPotion;
    } 
    
    void Start()
    {
        anim = GetComponent<Animator>();
        spriterend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        
        if (currentHealth > 0)
        {
            anim.SetTrigger("Attacked");
            StartCoroutine(Invunerability());
        }
        else
        {
            if(!dead)
            {
            anim.SetBool("isDead", true);
            GetComponent<PlayerMovement>().enabled = false;
            dead = true;
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Heal"))
        {
            if(currentPotion > 0)
            {
                AddHealth(Heal);
            
                currentPotion--;
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        anim.SetBool("isDead", false);
        anim.Play("Idle");
        StartCoroutine(Invunerability());
        transform.position = checkpoint.position;
        GetComponent<PlayerMovement>().enabled = true;
        currentPotion = startingPotion;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < FlashTimes; i++)
        {
            spriterend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
            spriterend.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (FlashTimes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }
}
