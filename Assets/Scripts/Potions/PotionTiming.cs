using UnityEngine;
using System.Collections.Generic;

/*
 * PotionTiming.cs — Map PotionData stats to minigame timers (static helpers)
 *
 * What lives here:
 *   - ResolveSecondsPerGame, ResolveRoundTotalSeconds from IsGreenedOut / GetMaxEffectValue.
 *   - Tunable constants (default seconds, green-out shortcuts, minimum per game).
 *
 * Main APIs / usage:
 *   - Minigame flow calls these with the active consumed / round potion; not used by inventory code.
 *   - ResolveReductionFromIntensity: tiered reduction from highest effect value (breakpoints scale with GreenOutThreshold vs legacy 1–16 scale).
 */

namespace CrossFade.Potions
{
    public static class PotionTiming
    {
        public const float DefaultSecondsPerGame = 15f;

        // Uncomment when a 4th minigame is in the round (gameIndex == 3).

        // public const float FourthGameSeconds = 12f;

        public const float MinimumSecondsPerGame = 5f;

        public const float DefaultRoundTotalSeconds = 45f;

        public const float GreenedOutSecondsPerGame = 5f;

        public const float GreenedOutRoundTotalSeconds = 30f;

        // Returns per-minigame duration based on effect values. (Add a gameIndex overload when a 4th minigame needs a fixed duration.)
        public static float ResolveSecondsPerGame(List<PotionData> potions)
        {
            if (potions == null || potions.Count == 0)
            {
                return DefaultSecondsPerGame;
            }

            var maxEffect = 0f;

            foreach (PotionData p in potions)
            {
                if (p.IsGreenedOut())
                {
                    return GreenedOutSecondsPerGame;
                }

                if (p.GetMaxEffectValue() > maxEffect)
                {
                    maxEffect = p.GetMaxEffectValue();
                }
            }

            var reduction = ResolveReductionFromIntensity(maxEffect);
            return Mathf.Max(DefaultSecondsPerGame - reduction, MinimumSecondsPerGame);
        }

        // Returns total round budget before returning to brewing.
        public static float ResolveRoundTotalSeconds(List<PotionData> potions)
        {
            if (potions == null || potions.Count == 0)
            {
                return DefaultRoundTotalSeconds;
            }

            foreach(PotionData p in potions)
            {
                if (p.IsGreenedOut())
                {
                    return GreenedOutRoundTotalSeconds;
                }
            }

            return DefaultRoundTotalSeconds;
        }

        public static float GetEffectValue(List<PotionData> potions)
        {
            if (potions == null || potions.Count == 0)
            {
                return 0f;
            }

            var maxEffect = 0f;

            foreach (PotionData p in potions)
            {
                if (p.IsGreenedOut())
                {
                    return -1;
                }

                if (p.GetMaxEffectValue() > maxEffect)
                {
                    maxEffect = p.GetMaxEffectValue();
                }
            }

            return maxEffect;
        }

        private static float ResolveReductionFromIntensity(float maxEffect)
        {
            var scale = PotionRules.GreenOutThreshold / 16f;
            if (maxEffect >= 14f * scale)
            {
                return 9f;
            }

            if (maxEffect >= 10f * scale)
            {
                return 6f;
            }

            if (maxEffect >= 7f * scale)
            {
                return 3f;
            }

            return 0f;
        }
    }
}

