using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * PotionRoller.cs — Procedural potion generation
 *
 * What it does:
 *   Picks a rarity via weighted random, then builds Name from Resources/PotionNaming.json
 *   (random affix + random suffix as "Affix Suffix"), and rolls every PotionRules.CoreEffects value 1–10.
 *
 * Main APIs / usage:
 *   - RollPotion(): entry point from PotionManager.TryRollAndStorePotion (manager may discard oldest when full).
 *   - RollRarity: weighted wheel using internal _rarityWeights.
 *   - BuildRolledPotion: creates PotionData with all six effects rolled.
 *   - SetRarityWeights: runtime balance/testing override.
 *   - RollCoreEffectValue: private 1–10 roll helper.
 */

namespace CrossFade.Potions
{
    // Responsible for rarity rolls, naming, and effect value rolling.
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

        // Rolls a complete potion instance (rarity, display name from JSON affix/suffix, core effects 1–10).
        public PotionData RollPotion()
        {
            var rarity = RollRarity();
            return BuildRolledPotion(rarity);
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

        // Creates a runtime potion instance for a rolled rarity.
        // Name = random affix + random suffix from JSON (display name).
        // Every core effect is procedurally rolled from 1..10.
        public PotionData BuildRolledPotion(PotionRarity rarity)
        {
            var affix = PotionNaming.PickRandomAffix(_namingData);
            var suffix = PotionNaming.PickRandomSuffix(_namingData);
            var potion = new PotionData
            {
                Affix = affix,
                Suffix = suffix,
                Name = $"{affix} {suffix}",
                Rarity = rarity
            };

            var effectTypes = PotionRules.CoreEffects;
            for (var i = 0; i < effectTypes.Length; i++)
            {
                var effectType = effectTypes[i];
                var rolledValue = RollCoreEffectValue();
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

        private static float RollCoreEffectValue()
        {
            return UnityEngine.Random.Range(PotionRules.MinRollValue, PotionRules.MaxRollValue + 1);
        }
    }
}


