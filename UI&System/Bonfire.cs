using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Bonfire : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; private set;}
    [SerializeField] private AudioClip cheackpointSound;

    // 【新增】：讓你在 Inspector 設定這個營火對應地圖的第幾個按鈕
    [Header("傳送點設定")]
    [SerializeField] private int areaIndex; 

    public PlayerHP playerHealth;
    public PlayerShoot Gun;
    public SaveController SC;   

    public void Rest()
    {
        playerHealth.MaxHp();
        Gun.Reload();
        SC.SaveGame();
        
        // 1. 儲存重生座標 (原本的邏輯)
        if (PlayerDataManager.instance != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            PlayerDataManager.instance.SetCheckpoint(currentScene, transform.position);
        }

        // 2. 【新增】：解鎖 UIManager 裡面的地圖傳送點
        if (UIManager.instance != null)
        {
            UIManager.instance.UnlockMapArea(areaIndex);
            Debug.Log($"營火已休息：已解鎖區域 {areaIndex}");
        }
    }

    public bool CanInteract()
    {
        // 如果你的營火是可以重複休息的，這裡可以考慮永遠回傳 true
        return true; 
    }

    public void Interact()
    {
        // 撥放音效 (如果有指定的話)
        if (cheackpointSound != null) AudioSource.PlayClipAtPoint(cheackpointSound, transform.position);

        Rest();
    }
}