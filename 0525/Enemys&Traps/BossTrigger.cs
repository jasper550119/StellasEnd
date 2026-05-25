using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public NPC startDialogueNPC;

    private bool hasTriggeredInThisScene = false;

    private void Start()
    {
        if (IsBossDefeated())
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || IsBossDefeated() || hasTriggeredInThisScene)
            return;

        hasTriggeredInThisScene = true;

        if (startDialogueNPC != null)
        {
            startDialogueNPC.StartDialogue();
        }
    }

    private bool IsBossDefeated()
    {
        return PlayerDataManager.instance != null && PlayerDataManager.instance.isBossDefeated;
    }
}
