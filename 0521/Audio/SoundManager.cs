using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [Header("核心設定")]
        [SerializeField] private SoundsSO SO;
        [SerializeField] private AudioMixer mixer; 
        public static SoundManager instance = null;
        private AudioSource audioSource; // 原本的，留給 SFX 使用
        
        // --- 新增：BGM 專用播放器與暫存 ---
        private AudioSource bgmSource;
        private SoundType? currentBGM = null;

        [Header("預設 Mixer 群組 (統一管理)")]
        [SerializeField] private AudioMixerGroup defaultMusicGroup;
        [SerializeField] private AudioMixerGroup defaultSFXGroup;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                Debug.LogError("SoundManager requires an AudioSource component.");

            // --- 新增：初始化 BGM 播放器 ---
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; // BGM 預設一定要循環
            bgmSource.playOnAwake = false;
        }

        public void ChangeMasterVolume(float value) => SetMixerVolume("MasterVolume", value);
        public void ChangeMusicVolume(float value) => SetMixerVolume("MusicVolume", value);
        public void ChangeSoundVolume(float value) => SetMixerVolume("SFXVolume", value);

        private void SetMixerVolume(string paramName, float sliderValue)
        {
            float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
            mixer.SetFloat(paramName, dbValue);
        }

        // 原本的 SFX 播放邏輯完全保留
        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
        {
            if (instance == null) return;
            if (instance.SO == null) return;
            
            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
            if (clips == null || clips.Length == 0) return;

            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            AudioMixerGroup targetGroup = soundList.mixer;

            if (targetGroup == null)
            {
                if (soundList.name.IndexOf("BGM", StringComparison.OrdinalIgnoreCase) >= 0 || 
                    soundList.name.IndexOf("Music", StringComparison.OrdinalIgnoreCase) >= 0)
                    targetGroup = instance.defaultMusicGroup;
                else
                    targetGroup = instance.defaultSFXGroup;
            }

            if (source)
            {
                source.outputAudioMixerGroup = targetGroup;
                source.clip = randomClip;
                source.volume = volume * soundList.volume;
                source.Play();
            }
            else
            {
                instance.audioSource.outputAudioMixerGroup = targetGroup;
                instance.audioSource.PlayOneShot(randomClip, volume * soundList.volume);
            }
        }

        // --- 新增：專門給 BGM 呼叫的方法 ---
        public static void PlayBGM(SoundType sound, float volume = 1)
        {
            if (instance == null || instance.SO == null) return;

            // 如果當前正在播同一首 BGM，直接跳過不重播（避免轉場時音樂中斷）
            if (instance.currentBGM == sound && instance.bgmSource.isPlaying) return;

            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
            if (clips == null || clips.Length == 0) return;

            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            AudioMixerGroup targetGroup = soundList.mixer ?? instance.defaultMusicGroup;

            instance.bgmSource.Stop(); // 停掉上一首
            instance.bgmSource.outputAudioMixerGroup = targetGroup;
            instance.bgmSource.clip = clip;
            instance.bgmSource.volume = volume * soundList.volume;
            instance.bgmSource.Play();

            instance.currentBGM = sound; // 記錄當前播放的 BGM
        }
    }

    [Serializable]
    public struct SoundList
    {
        public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;
    }
}