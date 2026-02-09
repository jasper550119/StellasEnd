using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private Animator anim;
    private PlayerMovement Move;
    private float maxClips = 6f;
    public float currentClips { get; private set; }

    private void Awake()
    {
        currentClips = maxClips;
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
        Move = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if(currentClips > 0)
            {
                anim.SetTrigger("Shoot");
                
                Shoot();
            
                currentClips--;
            }
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void Reload()
    {
        currentClips = maxClips;
    }
}
