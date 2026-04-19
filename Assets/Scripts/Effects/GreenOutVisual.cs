using UnityEngine;

namespace CrossFade.Potions
{
    public static class GreenOutVisual
    {
        public static float EvaluateMeter01(float maxStackedEffect)
        {
            if (maxStackedEffect <= 0f)
            {
                return 0f;
            }

            var cap = (float)PotionRules.GreenOutThreshold;
            var v = maxStackedEffect >= cap ? cap : maxStackedEffect;

            if (v <= 20f)
            {
                return Mathf.InverseLerp(0f, 20f, v) * 0.20f;
            }

            if (v <= 40f)
            {
                return Mathf.Lerp(0.20f, 0.45f, Mathf.InverseLerp(20f, 40f, v));
            }

            if (v <= 60f)
            {
                return Mathf.Lerp(0.45f, 0.82f, Mathf.InverseLerp(40f, 60f, v));
            }

            return Mathf.Lerp(0.82f, 1f, Mathf.InverseLerp(60f, cap, v));
        }
    }
}
