using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;

    [SerializeField] private Behaviour[] components;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
        }

        if(health <= 0)
        {
            anim.SetTrigger("Dead");
            Debug.Log("Enemy is dead");

            foreach (Behaviour component in components)
            {
                component.enabled = false;
            }
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
