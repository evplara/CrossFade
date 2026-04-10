using UnityEngine;

/*
 * PotionTiming.cs — Map potion stats to minigame timers (static helpers)
 *
 * What it does:
 *   Converts PotionData (especially IsGreenedOut / max effect strength) into per-minigame duration
 *   and total round time. Used by minigame flow code, not by inventory.
 *
 * Public API / usage:
 *   - ResolveSecondsPerGame(potion, gameIndex): green-out -> fixed short time; else reduce default time
 *     from max effect via ResolveReductionFromIntensity, clamp to minimum.
 *     (Fourth-minigame special case is commented out until a 4th minigame exists.)
 *   - ResolveRoundTotalSeconds(potion): green-out round cap vs default 45s.
 *   - Constants: tune in one place.
 *
 * Private:
 *   - ResolveReductionFromIntensity: tiered reduction from highest effect value.
 */

namespace CrossFade.Potions

{
    // Centralized mapping from potion effect intensity to minigame timing.

    public static class PotionTiming

    {
        public const float DefaultSecondsPerGame = 15f;

        // Uncomment when a 4th minigame is in the round (gameIndex == 3).

        // public const float FourthGameSeconds = 12f;

        public const float MinimumSecondsPerGame = 5f;

        public const float GreenedOutSecondsPerGame = 5f;

        public const float DefaultRoundTotalSeconds = 45f;

        public const float GreenedOutRoundTotalSeconds = 30f;

        // Returns per-minigame duration based on effect values and game slot index.
        public static float ResolveSecondsPerGame(PotionData potion, int gameIndex)
        {
            if (potion == null)
            {
                return DefaultSecondsPerGame;
            }

            if (potion.IsGreenedOut())
            {
                return GreenedOutSecondsPerGame;
            }

            // Fourth minigame (index 3): fixed 12s — enable when a 4th minigame ships.
            // if (gameIndex == 3)
            // {
            //     return FourthGameSeconds;
            // }

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


