using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    private void OnEnable()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.HealthChanged += UpdateText;
        }
    }

    private void OnDisable()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.HealthChanged -= UpdateText;
        }
    }

    private void Start()
    {
        UpdateText(HealthManager.Instance.CurrentHealth, HealthManager.Instance.MaxHealth);
    }

    //just going to use the current health
    private void UpdateText(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth.ToString();
    }
}
