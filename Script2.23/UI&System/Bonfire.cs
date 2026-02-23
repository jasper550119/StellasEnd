using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 【新增】引入場景管理

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
        
        // 【修改這裡】：將場景名稱與營火位置寫入 Manager，取代原本的 SetCheckpoint
        if (PlayerDataManager.instance != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            PlayerDataManager.instance.SetCheckpoint(currentScene, transform.position);
        }
    }

    public bool CanInteract()
    {
        return !IsUsed;
    }

    public void Interact()
    {
        if(!CanInteract()) return;
        Rest();
    }
}