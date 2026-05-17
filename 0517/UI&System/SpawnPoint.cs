using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("生成點專屬 ID (需與傳送門設定一致)")]
    public string spawnID;

    // 可選：在編輯器畫個小圖示，方便你在 Scene 視窗找到它
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}