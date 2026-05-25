using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace SmallHedge.SoundManager
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        private const string MasterVolumeKey = "Volume_Master";
        private const string MusicVolumeKey = "Volume_Music";
        private const string SFXVolumeKey = "Volume_SFX";

        [Header("Sound Data")]
        [SerializeField] private SoundsSO SO;
        [SerializeField] private AudioMixer mixer;

        [Header("Default Mixer Groups")]
        [SerializeField] private AudioMixerGroup defaultMusicGroup;
        [SerializeField] private AudioMixerGroup defaultSFXGroup;

        public static SoundManager instance = null;

        private AudioSource audioSource;
        private AudioSource bgmSource;
        private SoundType? currentBGM = null;

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

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            if (defaultMusicGroup != null)
            {
                bgmSource.outputAudioMixerGroup = defaultMusicGroup;
            }

            if (audioSource != null && defaultSFXGroup != null)
            {
                audioSource.outputAudioMixerGroup = defaultSFXGroup;
            }

            ApplySavedVolumes();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        public float MasterVolume => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        public float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        public float SFXVolume => PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        public void ChangeMasterVolume(float value)
        {
            SetVolume(MasterVolumeKey, "MasterVolume", value);
        }

        public void ChangeMusicVolume(float value)
        {
            SetVolume(MusicVolumeKey, "MusicVolume", value);
        }

        public void ChangeSoundVolume(float value)
        {
            SetVolume(SFXVolumeKey, "SFXVolume", value);
        }

        public void ApplySavedVolumes()
        {
            SetMixerVolume("MasterVolume", MasterVolume);
            SetMixerVolume("MusicVolume", MusicVolume);
            SetMixerVolume("SFXVolume", SFXVolume);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplySavedVolumes();
        }

        private void SetVolume(string prefsKey, string mixerParamName, float sliderValue)
        {
            float clampedValue = Mathf.Clamp01(sliderValue);
            PlayerPrefs.SetFloat(prefsKey, clampedValue);
            PlayerPrefs.Save();

            SetMixerVolume(mixerParamName, clampedValue);
        }

        private void SetMixerVolume(string paramName, float sliderValue)
        {
            if (mixer == null)
            {
                Debug.LogWarning($"SoundManager: AudioMixer is missing, cannot set {paramName}.");
                return;
            }

            float dbValue = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;

            if (!mixer.SetFloat(paramName, dbValue))
            {
                Debug.LogWarning($"SoundManager: Mixer parameter '{paramName}' is not exposed or does not exist.");
            }
        }

        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f)
        {
            if (instance == null || instance.SO == null) return;

            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
            if (clips == null || clips.Length == 0) return;

            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            AudioMixerGroup targetGroup = instance.GetTargetGroup(soundList);

            PlayClip(randomClip, source, volume * soundList.volume, targetGroup);
        }

        public static void PlayClip(AudioClip clip, AudioSource source = null, float volume = 1f)
        {
            if (instance == null || clip == null) return;

            PlayClip(clip, source, volume, instance.defaultSFXGroup);
        }

        private static void PlayClip(AudioClip clip, AudioSource source, float volume, AudioMixerGroup targetGroup)
        {
            if (instance == null || clip == null) return;

            if (source != null)
            {
                if (targetGroup != null)
                {
                    source.outputAudioMixerGroup = targetGroup;
                }
                source.PlayOneShot(clip, volume);
            }
            else if (instance.audioSource != null)
            {
                if (targetGroup != null)
                {
                    instance.audioSource.outputAudioMixerGroup = targetGroup;
                }
                instance.audioSource.PlayOneShot(clip, volume);
            }
        }

        public static void PlayBGM(SoundType sound, float volume = 1f)
        {
            if (instance == null || instance.SO == null || instance.bgmSource == null) return;

            if (instance.currentBGM == sound && instance.bgmSource.isPlaying) return;

            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
            if (clips == null || clips.Length == 0) return;

            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            AudioMixerGroup targetGroup = soundList.mixer ?? instance.defaultMusicGroup;

            instance.bgmSource.Stop();
            instance.bgmSource.outputAudioMixerGroup = targetGroup;
            instance.bgmSource.clip = clip;
            instance.bgmSource.volume = volume * soundList.volume;
            instance.bgmSource.Play();

            instance.currentBGM = sound;
        }

        private AudioMixerGroup GetTargetGroup(SoundList soundList)
        {
            if (soundList.mixer != null)
            {
                return soundList.mixer;
            }

            if (soundList.name.IndexOf("BGM", StringComparison.OrdinalIgnoreCase) >= 0 ||
                soundList.name.IndexOf("Music", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return defaultMusicGroup;
            }

            return defaultSFXGroup;
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
