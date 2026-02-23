using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;

    // 這個方法給 Timeline 的 Signal 呼叫，用來暫停
    public void PauseTimeline()
    {
        if (director != null)
            director.Pause();
    }

    // 這個方法給 NPC 的 OnDialogueEnd 呼叫，用來繼續
    public void ResumeTimeline()
    {
        if (director != null)
            director.Resume();
    }
}