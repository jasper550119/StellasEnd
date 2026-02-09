using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; private set;}
    [SerializeField] private AudioClip cheackpointSound;

    public PlayerHP playerHealth;
    public PlayerShoot Gun;
    public SaveController SC;   

    public void Rest()
    {
        playerHealth.MaxHp();
        Gun.Reload();
        SC.SaveGame();
        playerHealth.SetCheckpoint(transform);
    }

    public bool CanInteract()
    {
        return !IsUsed;
    }

    public void Interact()
    {
        if(!CanInteract())return;
        Rest();
    }
}
