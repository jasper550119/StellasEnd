using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequenceManager : MonoBehaviour
{
    [Header("UI 元素設定")]
    public GameObject endingUIContainer; 
    public CanvasGroup blackScreenGroup; 
    public CanvasGroup logoGroup;        

    [Header("互動鎖定 (防止黑畫面時再次互動或亂跑)")]
    public Collider2D finalNpcCollider;  // 【新增】關閉 NPC 的碰撞體，徹底斷絕互動可能
    public GameObject playerObject;      // 【新增】關閉玩家的控制腳本

    [Header("時間設定 (秒)")]
    public float fadeDuration = 1.5f;       
    public float logoDisplayDuration = 3.0f; 
    public float delayBeforeMainMenu = 1.0f; 

    private void Start()
    {
        if (endingUIContainer != null)
        {
            endingUIContainer.SetActive(false);
        }
    }

    /// <summary>
    /// 提供給最終 NPC 的 onDialogueEnd 事件呼叫
    /// </summary>
    public void PlayEndingSequence()
    {
        // 1. 關閉 NPC 碰撞體，防止玩家在黑畫面時再次觸發對話
        if (finalNpcCollider != null) 
        {
            finalNpcCollider.enabled = false;
        }

        // 2. 抓取並關閉玩家的操作腳本，讓玩家在看結局時乖乖站著
        if (playerObject != null)
        {
            var move = playerObject.GetComponent<PlayerMovement>();
            if (move != null) move.enabled = false;
            
            var attack = playerObject.GetComponent<PlayerAttack>();
            if (attack != null) attack.enabled = false;
            
            var shoot = playerObject.GetComponent<PlayerShoot>();
            if (shoot != null) shoot.enabled = false;
        }

        // 3. 打開 UI 容器，準備播放過場動畫
        if (endingUIContainer != null)
        {
            endingUIContainer.SetActive(true);
        }
        
        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator EndingCoroutine()
    {
        if (blackScreenGroup != null) blackScreenGroup.alpha = 0f;
        if (logoGroup != null) logoGroup.alpha = 0f;

        yield return StartCoroutine(Fade(blackScreenGroup, 0f, 1f, fadeDuration));
        yield return StartCoroutine(Fade(logoGroup, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(logoDisplayDuration);
        yield return StartCoroutine(Fade(logoGroup, 1f, 0f, fadeDuration));
        yield return new WaitForSeconds(delayBeforeMainMenu);

        if (UIManager.instance != null)
        {
            UIManager.instance.BackMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator Fade(CanvasGroup group, float start, float end, float duration)
    {
        if (group == null) yield break;

        float elapsed = 0f;
        group.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        group.alpha = end;
    }
}