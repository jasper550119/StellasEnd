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
        private AudioSource audioSource;

        [Header("預設 Mixer 群組 (統一管理)")]
        [SerializeField] private AudioMixerGroup defaultMusicGroup;
        [SerializeField] private AudioMixerGroup defaultSFXGroup;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
        }


        public void ChangeMasterVolume(float value) => SetMixerVolume("MasterVolume", value);
        public void ChangeMusicVolume(float value) => SetMixerVolume("MusicVolume", value);
        public void ChangeSoundVolume(float value) => SetMixerVolume("SFXVolume", value);

        private void SetMixerVolume(string paramName, float sliderValue)
        {
            float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
            mixer.SetFloat(paramName, dbValue);
        }

        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
        {
            if (instance == null)
            {
                Debug.LogError("場景中找不到 SoundManager 物件！請確保已將腳本掛載至 GameObject。");
                return;
            }

            if (instance.SO == null)
            {
                Debug.LogError("SoundManager 尚未指定 SoundsSO 資源檔案！");
                return;
            }
            
            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
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
    }

    // 關鍵：確保 SoundList 定義在 namespace 內
    [Serializable]
    public struct SoundList
    {
        public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;
    }
}