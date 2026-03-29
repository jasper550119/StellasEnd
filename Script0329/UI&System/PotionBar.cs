using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionBar : MonoBehaviour
{
    [SerializeField] private PlayerHP HP;
    [SerializeField] private Image totalpotion;
    [SerializeField] private Image currentpotion;

    void Start()
    {
        totalpotion.fillAmount = HP.currentPotion / 3;
    }

    // Update is called once per frame
    void Update()
    {
        currentpotion.fillAmount = HP.currentPotion / 3;
    }
}
