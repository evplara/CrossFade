using UnityEngine;
using TMPro;
using CrossFade.Potions;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PotionRollButton : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text potionNameText;
    public TMP_Text potionStatsText;

    [SerializeField] private List<PotionRaritySO> weights = new();
    [SerializeField] private List<GameObject> weightsButtons;
    private PotionRaritySO currenWeight;

    private void Start()
    {
        currenWeight = weights[0];
    }

    public void OnRollButtonPressed()
    {
        foreach(GameObject g in weightsButtons)
        {
            g.SetActive(false);
        }
        var controller = PotionController.Instance;
        if (controller == null)
        {
            Debug.LogError("PotionController instance not found!");
            return;
        }

        if (!controller.TryRollAndStorePotion(currenWeight))
        {
            Debug.LogWarning("Failed to roll potion (inventory full or not ready)");
            return;
        }

        var potion = controller.Inventory[^1];

        potionNameText.text = potion.Name;
        potionStatsText.text = BuildStatsString(potion);
    }

    private string BuildStatsString(PotionData potion)
    {
        float high = 0;
        float dizziness = 0;
        float nausea = 0;
        float hallucination = 0;
        float lethargy = 0;
        float focus = 0;

        foreach (var effect in potion.Effects)
        {
            switch (effect.EffectType)
            {
                case EffectType.High: high += effect.Value; break;
                case EffectType.Dizziness: dizziness += effect.Value; break;
                case EffectType.Nausea: nausea += effect.Value; break;
                case EffectType.Hallucination: hallucination += effect.Value; break;
                case EffectType.Lethargy: lethargy += effect.Value; break;
                case EffectType.Focus: focus += effect.Value; break;
            }
        }

        StringBuilder sb = new StringBuilder();

        Append(sb, "High", high);
        Append(sb, "Dizziness", dizziness);
        Append(sb, "Nausea", nausea);
        Append(sb, "Hallucination", hallucination);
        Append(sb, "Lethargy", lethargy);
        Append(sb, "Focus", focus);

        return sb.ToString();
    }

    private void Append(StringBuilder sb, string label, float value)
    {
        sb.AppendLine($"{label,-14}: {Format(value)}");
    }

    private string Format(float value)
    {
        return value switch
        {
            > 0 => $"+{value:0}",
            < 0 => $"{value:0}",
            _ => "0"
        };
    }

    public void SetWeights(PotionRaritySO rarity)
    {
        currenWeight = rarity;
    }
}