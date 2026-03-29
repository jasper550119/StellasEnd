using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBar : MonoBehaviour
{
    [SerializeField] private PlayerShoot Bullet;
    [SerializeField] private Image totalbullet;
    [SerializeField] private Image currentbullet;

    void Start()
    {
        totalbullet.fillAmount = Bullet.currentClips / 6;
    }

    // Update is called once per frame
    void Update()
    {
        currentbullet.fillAmount = Bullet.currentClips / 6;
    }
}
