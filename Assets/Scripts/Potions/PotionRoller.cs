using System;
using System.Collections.Generic;

/*
 * PotionRoller.cs — Procedural potion generation
 *
 * What lives here:
 *   - Weighted rarity roll, then Name / Affix / Suffix from PotionNaming (Resources JSON).
 *   - One rolled value per PotionRules.CoreEffects (inclusive range from PotionRaritySO → PotionData.EffectRollMin/Max).
 *
 * Main APIs / usage:
 *   - RollPotion / BuildRolledPotion: called from PotionController.TryRollAndStorePotion.
 *   - SetRarityWeights: optional runtime tuning for testing or balance.
 */

namespace CrossFade.Potions
{
    public class PotionRoller
    {
        private static readonly PotionRarity[] RarityOrder =
        {
            PotionRarity.Common,
            PotionRarity.Uncommon,
            PotionRarity.Rare,
            PotionRarity.Epic,
            PotionRarity.Legendary
        };

        private readonly Dictionary<PotionRarity, float> _rarityWeights = new();
        private readonly PotionNamingData _namingData;

        // Initializes default rarity weights and loads affix/suffix lists from Resources/PotionNaming.json.
        public PotionRoller(PotionNamingData namingData = null)
        {
            _namingData = namingData ?? PotionNaming.LoadFromResources();
            foreach (var r in RarityOrder)
            {
                _rarityWeights[r] = r switch
                {
                    PotionRarity.Common => 55f,
                    PotionRarity.Uncommon => 25f,
                    PotionRarity.Rare => 12f,
                    PotionRarity.Epic => 6f,
                    PotionRarity.Legendary => 2f,
                    _ => 1f
                };
            }
        }

        // Rolls a complete potion instance (rarity, display name from JSON affix/suffix, core effects in roll range).
        public PotionData RollPotion(PotionRaritySO weights = null)
        {
            var rarity = weights != null ? RollCustomRarity(weights) : RollRarity();
            var rollMin = weights != null ? weights.effectRollMin : PotionRules.MinRollValue;
            var rollMax = weights != null ? weights.effectRollMax : PotionRules.MaxRollValue;
            return BuildRolledPotion(rarity, rollMin, rollMax);
        }

        // Rolls a rarity tier based on configured weights.
        public PotionRarity RollRarity()
        {
            if (_rarityWeights.Count == 0)
            {
                throw new InvalidOperationException("Rarity weights are empty.");
            }

            var totalWeight = 0f;
            for (var i = 0; i < RarityOrder.Length; i++)
            {
                var r = RarityOrder[i];
                if (_rarityWeights.TryGetValue(r, out var w) && w > 0f)
                {
                    totalWeight += w;
                }
            }

            if (totalWeight <= 0f)
            {
                throw new InvalidOperationException("Total rarity weight must be positive.");
            }

            var roll = UnityEngine.Random.Range(0f, totalWeight);
            var cumulative = 0f;
            for (var i = 0; i < RarityOrder.Length; i++)
            {
                var r = RarityOrder[i];
                if (!_rarityWeights.TryGetValue(r, out var w) || w <= 0f)
                {
                    continue;
                }

                cumulative += w;
                if (roll <= cumulative)
                {
                    return r;
                }
            }

            return RarityOrder[RarityOrder.Length - 1];
        }

        public PotionRarity RollCustomRarity(PotionRaritySO weights)
        {
            if (weights.rarityValues.Length == 0)
            {
                throw new InvalidOperationException("Rarity weights are empty.");
            }

            var totalWeight = 0f;
            for (var i = 0; i < weights.rarityValues.Length; i++)
            {
                var r = weights.rarityValues[i];
                if (r.value > 0f)
                {
                    totalWeight += r.value;
                }
            }

            if (totalWeight <= 0f)
            {
                throw new InvalidOperationException("Total rarity weight must be positive.");
            }

            var roll = UnityEngine.Random.Range(0f, totalWeight);
            var cumulative = 0f;
            for (var i = 0; i < RarityOrder.Length; i++)
            {
                var r = weights.rarityValues[i];


                cumulative += r.value;
                if (roll <= cumulative)
                {
                    return r.rarity;
                }
            }

            return RarityOrder[RarityOrder.Length - 1];
        }

        // Creates a runtime potion instance for a rolled rarity.
        // Name = random affix + random suffix from JSON (display name).
        // Core effect values roll inclusively between effectRollMin and effectRollMax (stored on the potion).
        public PotionData BuildRolledPotion(PotionRarity rarity, int effectRollMin, int effectRollMax)
        {
            var affix = PotionNaming.PickRandomAffix(_namingData);
            var suffix = PotionNaming.PickRandomSuffix(_namingData);
            var potion = new PotionData
            {
                Affix = affix,
                Suffix = suffix,
                Name = $"{affix} {suffix}",
                Rarity = rarity,
                EffectRollMin = effectRollMin,
                EffectRollMax = effectRollMax
            };

            var effectTypes = PotionRules.CoreEffects;
            for (var i = 0; i < effectTypes.Length; i++)
            {
                var effectType = effectTypes[i];
                var rolledValue = RollCoreEffectValue(effectRollMin, effectRollMax);
                potion.Effects.Add(new PotionEffectValue(effectType, rolledValue));
            }

            return potion;
            // Output should look like:
            // PotionData(InstanceId=..., Name=..., Affix=..., Suffix=..., Rarity=..., Effects=..., IsConsumed=...)
        }

        // Sets or overrides rarity weights at runtime for balancing/testing.
        public void SetRarityWeights(Dictionary<PotionRarity, float> newWeights)
        {
            if (newWeights == null)
            {
                throw new ArgumentNullException(nameof(newWeights));
            }

            if (newWeights.Count == 0)
            {
                throw new ArgumentException("Weights cannot be empty.", nameof(newWeights));
            }

            _rarityWeights.Clear();
            foreach (var kvp in newWeights)
            {
                _rarityWeights[kvp.Key] = kvp.Value;
            }
        }

        private static float RollCoreEffectValue(int min, int max)
        {
            if (max < min)
            {
                (min, max) = (max, min);
            }

            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}


