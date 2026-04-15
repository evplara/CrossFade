using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using CrossFade.Potions;

public class PotionSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI selectedPotionText;
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Cup Visual")]
    [SerializeField] private SpriteRenderer liquidRenderer;
    [SerializeField] private Transform liquidTransform;

    [Header("Bottle Views")]
    [SerializeField] private PotionBottleView[] bottleViews;

    private Vector3 liquidStartScale;

    private PotionRoller roller;
    private PotionMixer mixer;
    private PotionManager manager;

    private void Start()
    {
        if (liquidTransform != null)
            liquidStartScale = liquidTransform.localScale;

        roller = new PotionRoller();
        mixer = new PotionMixer();
        manager = new PotionManager(roller, mixer, 3);

        manager.SetTemplates(CreateTemplates());

        RefreshScene();
    }

    public void RollPotion()
    {
        bool success = manager.TryRollAndStorePotion();

        if (!success)
        {
            debugText.text = "No free slot available.";
            return;
        }

        debugText.text = "Rolled a potion.";
        RefreshScene();
    }

    public void MixPotion0And1()
    {
        if (manager.Inventory.Count < 2)
        {
            debugText.text = "Need at least 2 potions to mix.";
            return;
        }

        manager.TryMixByIndex(0, 1);
        debugText.text = "Mixed potion 0 and potion 1.";
        RefreshScene();
    }

    public void ClearDisplay()
    {
        selectedPotionText.text = "No potion selected.";
        debugText.text = "Display cleared.";
    }

    public void SelectPotion(int index)
    {
        if (index < 0 || index >= manager.Inventory.Count)
        {
            selectedPotionText.text = "No potion in this slot.";
            return;
        }

        var potion = manager.Inventory[index];
        selectedPotionText.text = FormatPotionDetails(potion);
        UpdateCupVisual(potion);
    }

    private void RefreshScene()
    {
        RefreshInventoryText();
        RefreshBottleViews();

        if (manager.Inventory.Count > 0)
            SelectPotion(0);
        else
            ResetCupVisual();
    }

    private void RefreshInventoryText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Inventory");

        if (manager.Inventory.Count == 0)
        {
            sb.AppendLine("(empty)");
        }
        else
        {
            for (int i = 0; i < manager.Inventory.Count; i++)
            {
                var potion = manager.Inventory[i];
                sb.AppendLine($"{i}: {potion.Name} [{potion.Rarity}]");
            }
        }

        inventoryText.text = sb.ToString();
    }

    private void RefreshBottleViews()
    {
        for (int i = 0; i < bottleViews.Length; i++)
        {
            if (i < manager.Inventory.Count)
            {
                bottleViews[i].SetPotion(this, i, manager.Inventory[i]);
            }
            else
            {
                bottleViews[i].ClearPotion(this, i);
            }
        }
    }

    private string FormatPotionDetails(PotionData potion)
    {
        var sb = new StringBuilder();
        sb.AppendLine(potion.Name);
        sb.AppendLine($"Rarity: {potion.Rarity}");
        sb.AppendLine();

        for (int i = 0; i < potion.Effects.Count; i++)
        {
            var effect = potion.Effects[i];
            sb.AppendLine($"{effect.EffectType}: {effect.Value}");
        }

        sb.AppendLine();
        sb.AppendLine($"Max Effect: {potion.GetMaxEffectValue()}");
        sb.AppendLine($"Greened Out: {(potion.IsGreenedOut() ? "Yes" : "No")}");

        return sb.ToString();
    }

    private void UpdateCupVisual(PotionData potion)
    {
        if (liquidRenderer != null)
            liquidRenderer.color = BuildPotionColor(potion);

        if (liquidTransform != null)
        {
            float fill = Mathf.Clamp(0.35f + (potion.GetMaxEffectValue() / 20f), 0.35f, 1.2f);
            liquidTransform.localScale = new Vector3(liquidStartScale.x, fill, liquidStartScale.z);
        }
    }

    private void ResetCupVisual()
    {
        if (liquidRenderer != null)
            liquidRenderer.color = Color.clear;

        if (liquidTransform != null)
            liquidTransform.localScale = liquidStartScale;

        selectedPotionText.text = "No potion selected.";
    }

    private Color BuildPotionColor(PotionData potion)
    {
        float high = potion.GetEffectValue(EffectType.High) / PotionRules.MaxMixedValue;
        float dizzy = potion.GetEffectValue(EffectType.Dizziness) / PotionRules.MaxMixedValue;
        float focus = potion.GetEffectValue(EffectType.Focus) / PotionRules.MaxMixedValue;

        return new Color(
            Mathf.Clamp01(0.2f + high),
            Mathf.Clamp01(0.2f + focus),
            Mathf.Clamp01(0.2f + dizzy),
            1f
        );
    }

    private List<PotionTemplate> CreateTemplates()
    {
        return new List<PotionTemplate>
        {
            new PotionTemplate { TemplateId = "common_01", DisplayName = "Common Base", Rarity = PotionRarity.Common },
            new PotionTemplate { TemplateId = "uncommon_01", DisplayName = "Uncommon Base", Rarity = PotionRarity.Uncommon },
            new PotionTemplate { TemplateId = "rare_01", DisplayName = "Rare Base", Rarity = PotionRarity.Rare },
            new PotionTemplate { TemplateId = "epic_01", DisplayName = "Epic Base", Rarity = PotionRarity.Epic },
            new PotionTemplate { TemplateId = "legendary_01", DisplayName = "Legendary Base", Rarity = PotionRarity.Legendary }
        };
    }
}