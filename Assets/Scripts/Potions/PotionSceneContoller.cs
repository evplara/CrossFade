using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "PotionRoom2";
    [SerializeField] private float resultDisplayTime = 2f;

    [Header("Optional Custom Rarity Weights")]
    [SerializeField] private PotionRaritySO rarityWeights;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 3;

    private bool isTransitioning = false;
    private Vector3 liquidStartScale;

    private PotionRoller roller;
    private PotionMixer mixer;

    private readonly List<PotionData> inventory = new();

    private void Start()
    {
        if (liquidTransform != null)
            liquidStartScale = liquidTransform.localScale;

        roller = new PotionRoller();
        mixer = new PotionMixer();

        RefreshScene();
    }

    public void RollPotion()
    {
        if (inventory.Count >= maxSlots)
        {
            debugText.text = "No free slot available.";
            return;
        }

        PotionData newPotion = roller.RollPotion(rarityWeights);
        inventory.Add(newPotion);

        debugText.text = "Rolled a potion.";
        RefreshScene();
    }

    public void MixPotion0And1()
    {
        if (isTransitioning)
            return;

        if (inventory.Count < 2)
        {
            debugText.text = "Need at least 2 potions to mix.";
            return;
        }

        StartCoroutine(MixThenTransition());
    }

    private IEnumerator MixThenTransition()
    {
        isTransitioning = true;

        PotionData mixedPotion = mixer.Mix(inventory[0], inventory[1]);
        inventory[0] = mixedPotion;
        inventory.RemoveAt(1);

        debugText.text = "Mixed potion created!";
        RefreshScene();

        if (inventory.Count > 0)
        {
            SelectPotion(0);
        }

        yield return new WaitForSeconds(resultDisplayTime);

        SceneManager.LoadScene(nextSceneName);
    }

    public void ClearDisplay()
    {
        selectedPotionText.text = "No potion selected.";
        debugText.text = "Display cleared.";
    }

    public void SelectPotion(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            selectedPotionText.text = "No potion in this slot.";
            return;
        }

        var potion = inventory[index];
        selectedPotionText.text = FormatPotionDetails(potion);
        UpdateCupVisual(potion);
    }

    private void RefreshScene()
    {
        RefreshInventoryText();
        RefreshBottleViews();

        if (inventory.Count > 0)
            SelectPotion(0);
        else
            ResetCupVisual();
    }

    private void RefreshInventoryText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Inventory");

        if (inventory.Count == 0)
        {
            sb.AppendLine("(empty)");
        }
        else
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                var potion = inventory[i];
                sb.AppendLine($"{i}: {potion.Name} [{potion.Rarity}]");
            }
        }

        inventoryText.text = sb.ToString();
    }

    private void RefreshBottleViews()
    {
        for (int i = 0; i < bottleViews.Length; i++)
        {
            if (i < inventory.Count)
            {
                bottleViews[i].SetPotion(this, i, inventory[i]);
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
        const float colorNormalizeMax = 20f;

        float high = potion.GetEffectValue(EffectType.High) / colorNormalizeMax;
        float dizzy = potion.GetEffectValue(EffectType.Dizziness) / colorNormalizeMax;
        float focus = potion.GetEffectValue(EffectType.Focus) / colorNormalizeMax;

        return new Color(
            Mathf.Clamp01(0.2f + high),
            Mathf.Clamp01(0.2f + focus),
            Mathf.Clamp01(0.2f + dizzy),
            1f
        );
    }
}
