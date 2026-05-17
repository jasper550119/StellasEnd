using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("UI 設定")]
    [SerializeField] private GameObject doorUIPanel;

    public bool CanInteract()
    {
        // 確保有綁定 UI 面板，且 UI 面板目前是「關閉」狀態時，才允許玩家互動（顯示提示圖示）
        return doorUIPanel != null && !doorUIPanel.activeSelf;
    }

    public void Interact()
    {
        if (doorUIPanel != null)
        {
            doorUIPanel.SetActive(true);
            
            // 提示：因為 UI 開啟了，CanInteract 會變成 false
            // 玩家身上的 InteractionDetector 在這時不會再重複觸發它
        }
    }

    public void OpenDoor()
    {
        Debug.Log("門已成功解鎖開啟！");
        
        // 確保門被銷毀/隱藏時，UI 也要跟著關閉，避免卡在畫面上
        if (doorUIPanel != null)
        {
            doorUIPanel.SetActive(false);
        }

        gameObject.SetActive(false); 
    }
}