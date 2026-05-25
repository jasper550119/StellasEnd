using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private const string MasterSliderName = "MasterSlider";
    private const string MusicSliderName = "MusicSlider";
    private const string SFXSliderName = "SFXSlider";

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ResolveSliders();
        AddListeners();
        StartCoroutine(SyncWhenSoundManagerReady());
    }

    private void Start()
    {
        StartCoroutine(SyncWhenSoundManagerReady());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        RemoveListeners();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RebindSlidersAfterSceneLoad());
    }

    private void AddListeners()
    {
        RemoveListeners();
        ResolveSliders();

        if (_masterSlider != null) _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (_musicSlider != null) _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void RemoveListeners()
    {
        if (_masterSlider != null) _masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (_musicSlider != null) _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void ResolveSliders()
    {
        if (_masterSlider == null) _masterSlider = FindSceneSlider(MasterSliderName);
        if (_musicSlider == null) _musicSlider = FindSceneSlider(MusicSliderName);
        if (_sfxSlider == null) _sfxSlider = FindSceneSlider(SFXSliderName);
    }

    private Slider FindSceneSlider(string sliderName)
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Slider slider in sliders)
        {
            if (slider.name == sliderName && slider.gameObject.scene.IsValid() && slider.gameObject.scene.isLoaded)
            {
                return slider;
            }
        }

        return null;
    }

    private void SyncSlidersFromSavedVolume()
    {
        if (SoundManager.instance == null) return;

        ResolveSliders();
        SoundManager.instance.ApplySavedVolumes();

        if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(SoundManager.instance.MasterVolume);
        if (_musicSlider != null) _musicSlider.SetValueWithoutNotify(SoundManager.instance.MusicVolume);
        if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(SoundManager.instance.SFXVolume);
    }

    private IEnumerator SyncWhenSoundManagerReady()
    {
        yield return null;
        SyncSlidersFromSavedVolume();
    }

    private IEnumerator RebindSlidersAfterSceneLoad()
    {
        yield return null;

        RemoveListeners();
        _masterSlider = null;
        _musicSlider = null;
        _sfxSlider = null;

        ResolveSliders();
        AddListeners();
        SyncSlidersFromSavedVolume();
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (SoundManager.instance != null) SoundManager.instance.ChangeMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.instance != null) SoundManager.instance.ChangeMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.instance != null) SoundManager.instance.ChangeSoundVolume(value);
    }
}
