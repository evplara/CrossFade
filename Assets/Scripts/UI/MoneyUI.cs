using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        if (PlayerPotionStats.Instance != null)
        {
            PlayerPotionStats.Instance.MoneyChanged += UpdateMoney;
        }
    }

    private void OnDisable()
    {
        if (PlayerPotionStats.Instance != null)
        {
            PlayerPotionStats.Instance.MoneyChanged -= UpdateMoney;
        }
    }

    private void Awake()
    {
        if (PlayerPotionStats.Instance != null)
        {
            UpdateMoney();
        }
    }

    private void UpdateMoney()
    {
        moneyText.text = PlayerPotionStats.Instance.CurrentMoney.ToString();
    }
}
