using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CrossFade.Potions;

public class PotionRollButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private int costToBuy;
    [SerializeField] private PotionRaritySO rarityWeights;

    private void OnEnable()
    {
        if (PlayerPotionStats.Instance != null)
        {
            PlayerPotionStats.Instance.MoneyChanged += HandleButton;
        }
    }

    private void OnDisable()
    {
        if (PlayerPotionStats.Instance != null)
        {
            PlayerPotionStats.Instance.MoneyChanged -= HandleButton;
        }
    }

    private void Awake()
    {
        moneyLabel.text = costToBuy.ToString();
    }

    private void Start()
    {
        HandleButton();
    }

    void HandleButton()
    {
        if (costToBuy <= PlayerPotionStats.Instance.CurrentMoney)
        {
            button.interactable = true;
        }else
        {
            button.interactable = false;
        }
    }

    public void CallPotion()
    {
        PotionController.Instance.OnRollButtonClicked(rarityWeights);
        PlayerPotionStats.Instance.ChangeMoney(-costToBuy);
    }
}
