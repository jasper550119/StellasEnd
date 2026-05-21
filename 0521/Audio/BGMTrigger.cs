using UnityEngine;
using SmallHedge.SoundManager;

public class BGMTrigger : MonoBehaviour
{
    public enum BGMType { MainMenu, Boss, Default }
    
    [Header("選擇此場景要播放的 BGM 類型")]
    [SerializeField] private BGMType sceneBGM;

    private void Start()
    {
        // 根據檢查器選的類型，播放對應的音效
        switch (sceneBGM)
        {
            case BGMType.MainMenu:
                SoundManager.PlayBGM(SoundType.MainMenuBGM);
                break;
            case BGMType.Boss:
                SoundManager.PlayBGM(SoundType.BossBGM);
                break;
            case BGMType.Default:
                SoundManager.PlayBGM(SoundType.DefaultBGM);
                break;
        }
    }
}