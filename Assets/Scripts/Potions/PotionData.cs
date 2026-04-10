using System;
using System.Collections.Generic;

/*
 * PotionData.cs — Core potion schema and runtime data
 *
 * What lives here:
 *   - PotionRarity: tier enum for weighted rolls and display.
 *   - EffectType: all six gameplay effects (High, Dizziness, Nausea, Hallucination, Lethargy, Focus).
 *   - PotionRules: shared constants (roll 1–10, green-out threshold, ordered CoreEffects list).
 *   - PotionEffectValue: one effect type + numeric value on an instance.
 *   - PotionData: runtime potion (Name, Affix, Suffix, Rarity, Effects, consumed flag). Rolled names are
 *     "Affix Suffix"; mixed names are "Affix + Suffix" from left/right parts (see PotionMixer).
 *
 * Main APIs / usage:
 *   - PotionRoller builds PotionData; PotionMixer merges two; PotionManager stores them.
 *   - GetEffectValue / HasEffect: read one stat; GetMaxEffectValue / IsGreenedOut: minigame timing & VFX thresholds.
 *   - Tooltip or HUD: iterate Effects or use GetEffectValue per EffectType.
 */

namespace CrossFade.Potions
{
    // Rarity tiers used for weighted potion rolling and value scaling.
    public enum PotionRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    // Core effect catalog used by rolling, mixing, and minigame behavior.
    public enum EffectType
    {
        High,
        Dizziness,
        Nausea,
        Hallucination,
        Lethargy,
        Focus
    }

    // Shared numeric tuning for potion generation and effect checks.
    public static class PotionRules
    {
        public const int MinRollValue = 1;
        public const int MaxRollValue = 10;
        public const int MaxMixedValue = 20;
        public const int GreenOutThreshold = 16;

        public static readonly EffectType[] CoreEffects =
        {
            EffectType.High,
            EffectType.Dizziness,
            EffectType.Nausea,
            EffectType.Hallucination,
            EffectType.Lethargy,
            EffectType.Focus
        };

        // Enum order ascending; used by rarity weights and rolls.
        public static readonly PotionRarity[] RarityOrder =
        {
            PotionRarity.Common,
            PotionRarity.Uncommon,
            PotionRarity.Rare,
            PotionRarity.Epic,
            PotionRarity.Legendary
        };
    }

    [Serializable]
    public class PotionEffectValue
    {
        public EffectType EffectType;
        public float Value;

        public PotionEffectValue()
        {
        }

        public PotionEffectValue(EffectType effectType, float value)
        {
            EffectType = effectType;
            Value = value;
        }
    }

    [Serializable]
    public class PotionData
    {
        public string InstanceId = Guid.NewGuid().ToString();
        public string Name = string.Empty;
        public string Affix = string.Empty;
        public string Suffix = string.Empty;
        public PotionRarity Rarity = PotionRarity.Common;
        public List<PotionEffectValue> Effects = new();
        public bool IsConsumed;

        public PotionData()
        {
        }

        public PotionData(IReadOnlyList<PotionEffectValue> effects, PotionRarity rarity, string name)
            : this(effects, rarity, name, string.Empty, string.Empty)
        {
        }

        public PotionData(IReadOnlyList<PotionEffectValue> effects, PotionRarity rarity, string name, string affix, string suffix)
        {
            Rarity = rarity;
            Name = name ?? string.Empty;
            Affix = affix ?? string.Empty;
            Suffix = suffix ?? string.Empty;
            if (effects == null)
            {
                Effects = new List<PotionEffectValue>();
                return;
            }

            Effects = new List<PotionEffectValue>(effects.Count);
            for (var i = 0; i < effects.Count; i++)
            {
                var e = effects[i];
                Effects.Add(new PotionEffectValue(e.EffectType, e.Value));
            }
        }

        public bool TryGetEffect(EffectType effectType, out float value)
        {
            value = 0f;
            if (Effects == null)
            {
                return false;
            }

            for (var i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].EffectType == effectType)
                {
                    value = Effects[i].Value;
                    return true;
                }
            }

            return false;
        }

        public bool HasEffect(EffectType effectType) => TryGetEffect(effectType, out _);

        public float GetEffectValue(EffectType effectType) =>
            TryGetEffect(effectType, out var v) ? v : 0f;
        
        public float GetMaxEffectValue()
        {
            if (Effects == null || Effects.Count == 0)
            {
                return 0f;
            }

            var max = 0f;
            for (var i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].Value > max)
                {
                    max = Effects[i].Value;
                }
            }

            return max;
        }

        public bool IsGreenedOut() =>
            GetEffectValue(EffectType.High) >= PotionRules.GreenOutThreshold
            || GetMaxEffectValue() >= PotionRules.GreenOutThreshold;
    }
}
