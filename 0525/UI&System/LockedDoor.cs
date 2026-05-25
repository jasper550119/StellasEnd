using System.Collections;
using SmallHedge.SoundManager;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    public string targetSceneName;
    public string targetSpawnID;

    [Header("Sound")]
    public AudioClip successSound;
    public AudioClip failSound;

    private AudioSource audioSource;
    private bool isLoading = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public bool CanInteract()
    {
        return !isLoading;
    }

    public void Interact()
    {
        if (isLoading) return;

        if (PlayerDataManager.instance != null && PlayerDataManager.instance.hasBossKey)
        {
            StartCoroutine(TransitionRoutine());
        }
        else
        {
            PlaySound(failSound);
            Debug.Log("You need the Boss key to open this door.");
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isLoading = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHP playerHP = player.GetComponent<PlayerHP>();
            PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

            if (playerHP != null && playerShoot != null && PlayerDataManager.instance != null)
            {
                PlayerDataManager.instance.SavePlayerData(playerHP.currentHealth, playerHP.currentPotion, playerShoot.currentClips);
            }
        }

        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.isRespawning = false;
            PlayerDataManager.instance.isFastTraveling = false;
            PlayerDataManager.instance.isTransitioning = true;
            PlayerDataManager.instance.targetSpawnID = targetSpawnID;
        }

        PlaySound(successSound);

        float delayTime = successSound != null ? successSound.length : 0.5f;
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(targetSceneName);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            SoundManager.PlayClip(clip, audioSource);
        }
    }
}
