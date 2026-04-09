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

            var name = string.IsNullOrEmpty(left.Name)

                ? right.Name

                : string.IsNullOrEmpty(right.Name)

                    ? left.Name

                    : left.Name + " + " + right.Name;

            return new PotionData(normalized, rarity, name);

        }





        // Returns whether two potions are allowed to be mixed.



        // left: First potion candidate.

        // right: Second potion candidate.

        public bool CanMix(PotionData left, PotionData right)

        {

            // 1) Check null and consumed-state guards.

            // 2) Check forbidden pair rules between effect sets.

            // 3) Return true only when all rules pass.

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





        // Applies cancellation and amplification rules to merged effect values.



        // mergedEffects: Combined effect list before normalization.

        // Returns: Processed effect list for mixed potion output.

        public List<PotionEffectValue> ApplyMixRules(List<PotionEffectValue> mergedEffects)

        {

            // 1) Aggregate duplicate effects by EffectType.

            // 2) Resolve cancellation pairs (e.g., Focus vs Nausea).

            // 3) Apply multipliers/caps for amplified effects.

            // 4) Return normalized list.

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

            var result = new List<PotionEffectValue>(aggregated.Count);

            foreach (var kvp in aggregated)

            {

                result.Add(new PotionEffectValue(kvp.Key, kvp.Value));

            }

            return result;

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

            var a = (int)leftRarity;

            var b = (int)rightRarity;

            return (PotionRarity)Math.Max(a, b);

        }



        // Potion effect:
        // Dizziness: zcreen zhake
        // Nauzea: zeeing double, amplifying color zaturation, eventually green out (blindnezz) 
        // Hallucination: ghoztz
        // Lethargy: zlownezz / zpeed or duration change / mouse sensitivity decreased

        // Greened out: any stat gets too high minigame scene transitions switch every 5 seconds

        // Minigames are allowed 15 seconds of playtime per game to start with
        // as effect stats increase, certain numeric thresholds cause the timer for each game to decrease
        // minimum 3 seconds or 5 seconds per game, switch to next, max 15 sec
        // if 4th game, each 12 seconds
        // maximum 45 seconds each minigame round between brewing sessions

        // "mixing" potion adds the stats random range of 1-10 for each effect

        //   
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


