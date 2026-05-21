using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public PlayerMovement playerMovement;
    public CamMovement camMovement;

    public void PauseTimeline()
    {
        if (director != null && director.playableGraph.IsValid())
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            if (camMovement != null) camMovement.isFollowing = false;
        }
    }

    public void ResumeTimeline()
    {
        if (director != null && director.playableGraph.IsValid())
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }

    public void OnCutsceneEnd()
    {
    // 處理玩家...
    if (playerMovement != null)
    {
        playerMovement.SetControl(true);
    }

    // 處理攝影機：直接呼叫工具類
    if (camMovement != null && playerMovement != null)
    {
        // 傳入你的攝影機腳本與玩家的 Transform
        CameraUnclenchTool.ForceUnclench(camMovement, playerMovement.transform);
    }
}
}
