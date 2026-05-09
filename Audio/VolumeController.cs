using UnityEngine;
using UnityEngine.UI;

// 確保有引用 SoundManager 的 Namespace
using SmallHedge.SoundManager;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Start()
    {
        // 監聽事件，數值改變時呼叫 SoundManager
        if(_masterSlider) _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if(_musicSlider) _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if(_sfxSlider) _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 初始化設定 (可選：設定預設值為 1，即最大聲)
        // 注意：這裡直接呼叫一次，確保遊戲開始時音量是 Slider 的位置
        if(_masterSlider) OnMasterVolumeChanged(_masterSlider.value);
        if(_musicSlider) OnMusicVolumeChanged(_musicSlider.value);
        if(_sfxSlider) OnSFXVolumeChanged(_sfxSlider.value);
    }

    public void OnMasterVolumeChanged(float value)
    {
        // 呼叫 SoundManager 的轉換方法
        SoundManager.instance.ChangeMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        SoundManager.instance.ChangeMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SoundManager.instance.ChangeSoundVolume(value);
    }
}