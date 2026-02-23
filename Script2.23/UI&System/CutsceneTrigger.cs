using UnityEngine;
using UnityEngine.Playables; // 必須引用這個才能控制 Timeline

public class CutsceneTrigger : MonoBehaviour
{
    [Header("要播放的過場動畫")]
    public PlayableDirector cutsceneDirector;

    [Header("是否只播放一次")]
    public bool playOnce = true;

    // 當有物體進入 Trigger 範圍時觸發
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查進入的是不是玩家 (記得確認你的玩家物件 Tag 是 "Player")
        if (other.CompareTag("Player"))
        {
            // 播放 Timeline
            if (cutsceneDirector != null)
            {
                cutsceneDirector.Play();
            }

            // 如果設定只播放一次，就關閉這個觸發器
            if (playOnce)
            {
                gameObject.SetActive(false);
            }
        }
    }
}