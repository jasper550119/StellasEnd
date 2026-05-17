using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("開場對話 NPC 系統")]
    public NPC startDialogueNPC;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 觸發對話
            if (startDialogueNPC != null)
            {
                startDialogueNPC.StartDialogue();
            }
            
            // 觸發後關閉這個碰撞體，避免重複觸發
            gameObject.SetActive(false);
        }
    }
}