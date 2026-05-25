using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName;

    [Header("Target Spawn ID")]
    public string targetSpawnID;

    private bool isLoading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLoading || !other.CompareTag("Player"))
            return;

        isLoading = true;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning($"SceneTransition on '{name}' has no next scene set.");
            isLoading = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSpawnID))
        {
            Debug.LogWarning($"SceneTransition from '{SceneManager.GetActiveScene().name}' to '{nextSceneName}' has no target spawn ID set.");
        }

        PlayerHP playerHP = other.GetComponent<PlayerHP>();
        PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();

        if (playerHP != null && playerShoot != null && PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.SavePlayerData(playerHP.currentHealth, playerHP.currentPotion, playerShoot.currentClips);

            PlayerDataManager.instance.isRespawning = false;
            PlayerDataManager.instance.isFastTraveling = false;
            PlayerDataManager.instance.isTransitioning = true;
            PlayerDataManager.instance.targetSpawnID = targetSpawnID;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
