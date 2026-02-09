using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject playerStats;
    [SerializeField] private GameObject mapPage;
    [SerializeField] private GameObject volumeSetting;

    [Header("Player References")]
    [SerializeField] private GameObject playerObject; 
    
    private PlayerMovement moveScript;
    private PlayerAttack attackScript;
    private PlayerShoot shootScript;
    private bool isPaused;
    private bool isMaped;

    void Start()
    {
        if (playerObject != null)
        {
            moveScript = playerObject.GetComponent<PlayerMovement>();
            attackScript = playerObject.GetComponent<PlayerAttack>();
            shootScript = playerObject.GetComponent<PlayerShoot>();
        }
        else
        {
            moveScript = GetComponent<PlayerMovement>();
            attackScript = GetComponent<PlayerAttack>();
            shootScript = GetComponent<PlayerShoot>();
            
            if(moveScript == null) Debug.LogError("找不到 PlayerMovement！請確認 UIManager 是否掛在主角身上，或是忘記在 Inspector 指定 Player Object。");
        }

        isPaused = false;
        isMaped = false;
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (playerStats != null) playerStats.SetActive(true);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (Input.GetButtonDown("Map") && !isPaused)
        {
            if (isMaped)
                ResumeGame();
            else   
                OpenMap();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        
        if (pauseScreen != null) pauseScreen.SetActive(true);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);

        if (moveScript != null) moveScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
        if (shootScript != null) shootScript.enabled = false;
    }

    public void OpenMap()
    {
        isMaped = true;

        if (mapPage != null) mapPage.SetActive(true);

        if (moveScript != null) moveScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
        if (shootScript != null) shootScript.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;

        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (playerStats != null) playerStats.SetActive(true);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);

        if (moveScript != null) moveScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
        if (shootScript != null) shootScript.enabled = true;
    }

    public void Vsetting()
    {
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(true);
    }

    public void BackPause()
    {
        if (pauseScreen != null) pauseScreen.SetActive(true);
        if (playerStats != null) playerStats.SetActive(false);
        if (mapPage != null) mapPage.SetActive(false);
        if (volumeSetting != null) volumeSetting.SetActive(false);
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}