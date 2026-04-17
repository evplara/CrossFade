using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnDisable()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.MoneyChanged -= UpdateMoney;
        }
    }

    private void Start()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.MoneyChanged += UpdateMoney;
        }

        if (MoneyManager.Instance != null)
        {
            UpdateMoney();
        }
    }

    private void UpdateMoney()
    {
        moneyText.text = MoneyManager.Instance.CurrentMoney.ToString();
    }
}
