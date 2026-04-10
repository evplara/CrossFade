using System;
using System.Collections.Generic;

/*
 * PotionMixer.cs — Combine two potions into one
 *
 * What it does:
 *   Merges effect lists from left + right, sums values per EffectType, outputs one PotionData
 *   with all core effect types present (missing types become 0).
 *
 * Main APIs / usage:
 *   - Mix(left, right): validates inputs, merges, applies ApplyMixRules, picks max rarity; mixed Name is
 *     left.Affix + " + " + right.Suffix (fallback to full Name if parts missing).
 *   - CanMix: null/consumed guard + ViolatesForbiddenRules (currently always false - extend for forbidden pairs).
 *   - ApplyMixRules: aggregate duplicates then emit ordered list from PotionRules.CoreEffects.
 *   - ResolveMixedRarity: max of the two rarities.
 *   - Called from PotionManager.TryMixByIndex only.
 */

namespace CrossFade.Potions
{
    // Handles potion combination rules: cancellation, amplification, and conflicts.
    public class PotionMixer
    {
        // Attempts to mix two potions into a new result potion instance.
        public PotionData Mix(PotionData left, PotionData right)
        {
            if (left == null || right == null)
            {
                throw new ArgumentNullException(left == null ? nameof(left) : nameof(right));
            }

            if (left.IsConsumed || right.IsConsumed)
            {
                throw new ArgumentException("Cannot mix consumed potions.");
            }

            if (!CanMix(left, right))
            {
                throw new ArgumentException("These potions cannot be mixed.");
            }

            var mergedEffects = new List<PotionEffectValue>();
            AddEffects(mergedEffects, left.Effects);
            AddEffects(mergedEffects, right.Effects);

            var normalized = ApplyMixRules(mergedEffects);
            var rarity = ResolveMixedRarity(left.Rarity, right.Rarity);

            var affix = !string.IsNullOrWhiteSpace(left.Affix) ? left.Affix.Trim() : left.Name.Trim();
            var suffix = !string.IsNullOrWhiteSpace(right.Suffix) ? right.Suffix.Trim() : right.Name.Trim();
            var mixName = $"{affix} + {suffix}";

            return new PotionData(normalized, rarity, mixName, affix, suffix);
        }

        // Returns whether two potions are allowed to be mixed.
        public bool CanMix(PotionData left, PotionData right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            if (left.IsConsumed || right.IsConsumed)
            {
                return false;
            }

            return !ViolatesForbiddenRules(left, right);
        }

        // Applies additive mix rules to merged effect values.
        // New spec: values are directly added per effect type
        public List<PotionEffectValue> ApplyMixRules(List<PotionEffectValue> mergedEffects)
        {
            if (mergedEffects == null || mergedEffects.Count == 0)
            {
                return new List<PotionEffectValue>();
            }

            var aggregated = new Dictionary<EffectType, float>();
            for (var i = 0; i < mergedEffects.Count; i++)
            {
                var e = mergedEffects[i];
                if (aggregated.TryGetValue(e.EffectType, out var sum))
                {
                    aggregated[e.EffectType] = sum + e.Value;
                }
                else
                {
                    aggregated[e.EffectType] = e.Value;
                }
            }

            var result = new List<PotionEffectValue>(PotionRules.CoreEffects.Length);
            var effectTypes = PotionRules.CoreEffects;
            for (var i = 0; i < effectTypes.Length; i++)
            {
                var effectType = effectTypes[i];
                aggregated.TryGetValue(effectType, out var baseValue);
                result.Add(new PotionEffectValue(effectType, baseValue));
            }

            return result;
        }

        // Computes result rarity from two source potion rarities.
        public PotionRarity ResolveMixedRarity(PotionRarity leftRarity, PotionRarity rightRarity)
        {
            var a = (int)leftRarity;
            var b = (int)rightRarity;
            return (PotionRarity)Math.Max(a, b);
        }

        private static void AddEffects(List<PotionEffectValue> target, List<PotionEffectValue> source)
        {
            if (target == null || source == null)
            {
                return;
            }

            for (var i = 0; i < source.Count; i++)
            {
                var e = source[i];
                target.Add(new PotionEffectValue(e.EffectType, e.Value));
            }
        }

        private static bool ViolatesForbiddenRules(PotionData left, PotionData right)
        {
            return false;
        }
    }
}


