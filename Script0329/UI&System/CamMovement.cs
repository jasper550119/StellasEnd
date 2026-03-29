using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float speed = 5f;
    
    public bool isFollowing = true; 
    public float maxCatchUpDistance = 3f; 

    [Header("Camera Bounds")]
    public bool useBounds = false; 
    public Vector2 minBounds;      
    public Vector2 maxBounds;      

    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect; 
    }

    void LateUpdate()
    {
        if (!isFollowing || player == null) return;

        // 1 & 2. 計算平滑跟隨位置
        Vector3 desiredPos = player.position + offset;
        desiredPos.z = transform.position.z; 
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, speed * Time.deltaTime);

        // 3. 防護機制 (防止跑出畫面)
        float currentDistance = Vector2.Distance(smoothedPos, desiredPos);
        if (currentDistance > maxCatchUpDistance)
        {
            Vector2 direction = (smoothedPos - desiredPos).normalized;
            smoothedPos = desiredPos + (Vector3)(direction * maxCatchUpDistance);
            smoothedPos.z = transform.position.z; 
        }

        // 4. 邊界限制處理
        if (useBounds)
        {
            float clampedX = Mathf.Clamp(smoothedPos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            float clampedY = Mathf.Clamp(smoothedPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
            smoothedPos = new Vector3(clampedX, clampedY, smoothedPos.z);
        }

        // 5. 更新攝影機位置
        transform.position = smoothedPos;
    }

    // ⭐ 新增：在 Scene 視窗繪製輔助線
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            // 設定輔助線的顏色為紅色 (您可以改成喜歡的顏色)
            Gizmos.color = Color.red;

            // 計算邊界框的中心點
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                (minBounds.y + maxBounds.y) / 2f,
                transform.position.z // 保持在攝影機的 Z 軸深度
            );

            // 計算邊界框的長寬尺寸
            Vector3 size = new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y,
                0.1f // Z 軸厚度給一點點即可
            );

            // 畫出一個線框方塊代表邊界
            Gizmos.DrawWireCube(center, size);
        }
    }
}