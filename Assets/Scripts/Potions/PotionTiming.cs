using UnityEngine;

/*
 * PotionTiming.cs — Map PotionData stats to minigame timers (static helpers)
 *
 * What lives here:
 *   - ResolveSecondsPerGame, ResolveRoundTotalSeconds from IsGreenedOut / GetMaxEffectValue.
 *   - Tunable constants (default seconds, green-out shortcuts, minimum per game).
 *
 * Main APIs / usage:
 *   - Minigame flow calls these with the active consumed / round potion; not used by inventory code.
 *   - ResolveReductionFromIntensity: private tiered reduction from highest effect value.
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

        public const float GreenedOutSecondsPerGame = 3f;

        public const float GreenedOutRoundTotalSeconds = 30f;

        // Returns per-minigame duration based on effect values. (Add a gameIndex overload when a 4th minigame needs a fixed duration.)
        public static float ResolveSecondsPerGame(PotionData potion)
        {
            if (potion == null)
            {
                return DefaultSecondsPerGame;
            }

            if (potion.IsGreenedOut())
            {
                return GreenedOutSecondsPerGame;
            }

            var maxEffect = potion.GetMaxEffectValue();
            var reduction = ResolveReductionFromIntensity(maxEffect);
            return Mathf.Max(DefaultSecondsPerGame - reduction, MinimumSecondsPerGame);
        }

        // Returns total round budget before returning to brewing.
        public static float ResolveRoundTotalSeconds(PotionData potion)
        {
            if (potion != null && potion.IsGreenedOut())
            {
                return GreenedOutRoundTotalSeconds;
            }
            return DefaultRoundTotalSeconds;

        }

        private static float ResolveReductionFromIntensity(float maxEffect)
        {
            if (maxEffect >= 14f)
            {
                return 9f;
            }

            if (maxEffect >= 10f)
            {
                return 6f;
            }

            if (maxEffect >= 7f)
            {
                return 3f;
            }

            return 0f;
        }
    }
}

