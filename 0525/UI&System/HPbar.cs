using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    [SerializeField] private PlayerHP playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Start()
    {
        if (totalhealthBar != null)
        {
            totalhealthBar.fillAmount = 1f;
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (playerHealth == null || currenthealthBar == null)
            return;

        float maxHealth = Mathf.Max(playerHealth.maxHealth, 0.0001f);
        currenthealthBar.fillAmount = Mathf.Clamp01(playerHealth.currentHealth / maxHealth);
    }
}
