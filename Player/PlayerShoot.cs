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

        if (PlayerDataManager.instance != null)
        {
            if (PlayerDataManager.instance.isRespawning)
            {
                // 跨場景復活時，子彈補滿
                currentClips = maxClips; 
            }
            else if (PlayerDataManager.instance.isFastTraveling || PlayerDataManager.instance.hasSavedData)
            {
                // 傳送或過門時，繼承子彈
                currentClips = PlayerDataManager.instance.savedClips;
            }
        }
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