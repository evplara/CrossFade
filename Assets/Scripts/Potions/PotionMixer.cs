using System;
using System.Collections.Generic;

namespace CrossFade.Potions
{
    // Handles potion combination rules: cancellation, amplification, and conflicts.
    public class PotionMixer
    {

        // Attempts to mix two potions into a new result potion instance.

        // left: First source potion.
        // right: Second source potion.
        // Returns: Mixed potion result.
        public PotionData Mix(PotionData left, PotionData right)
        {
            // 1) Validate both inputs are non-null and not consumed.
            // 2) Reject forbidden effect combinations.
            // 3) Merge effects from both potions into a working list.
            // 4) Apply cancellation/amplification rules.
            // 5) Build and return final PotionData result.
            throw new NotImplementedException();
        }


        // Returns whether two potions are allowed to be mixed.

        // left: First potion candidate.
        // right: Second potion candidate.
        public bool CanMix(PotionData left, PotionData right)
        {
            // 1) Check null and consumed-state guards.
            // 2) Check forbidden pair rules between effect sets.
            // 3) Return true only when all rules pass.
            throw new NotImplementedException();
        }


        // Applies cancellation and amplification rules to merged effect values.

        // mergedEffects: Combined effect list before normalization.
        // Returns: Processed effect list for mixed potion output.
        public List<PotionEffectValue> ApplyMixRules(List<PotionEffectValue> mergedEffects)
        {
            // 1) Aggregate duplicate effects by EffectType.
            // 2) Resolve cancellation pairs (e.g., Focus vs Nausea).
            // 3) Apply multipliers/caps for amplified effects.
            // 4) Return normalized list.
            throw new NotImplementedException();
        }


        // Computes result rarity from two source potion rarities.

        // leftRarity: Rarity from first potion.
        // rightRarity: Rarity from second potion.
        // Returns: Output rarity for mixed potion.
        public PotionRarity ResolveMixedRarity(PotionRarity leftRarity, PotionRarity rightRarity)
        {
            // 1) Choose baseline (max, avg, or rule-table approach).
            // 2) Optionally apply downgrade/upgrade chance rules.
            // 3) Return final rarity.
            throw new NotImplementedException();
        }
    }
}
