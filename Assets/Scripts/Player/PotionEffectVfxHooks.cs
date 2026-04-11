using CrossFade.Potions;
using UnityEngine;

// Central place to plug VFX / audio / post-processing per effect type and strength.
// Call <see cref="ApplyAllFromPlayerStats"/> from minigame scenes after load (totals come from <see cref="PlayerPotionStats"/>).
public static class PotionEffectVfxHooks
{
    // Optional: skip applying when total is at or below this (per effect).
    public const float MinEffectEpsilon = 0.001f;

    public static void ApplyAllFromPlayerStats()
    {
        var stats = PlayerPotionStats.Instance;
        if (stats == null)
        {
            Debug.LogWarning("[PotionEffectVfxHooks] No PlayerPotionStats in scene (drink in PotionRoom first, or add a persistent stats object).");
            return;
        }

        foreach (var effectType in PotionRules.CoreEffects)
        {
            float value = stats.GetTotal(effectType);
            ApplySingle(effectType, value);
        }
    }

    // One effect type per call. ApplyAllFromPlayerStats loops all CoreEffects, so multiple types (High + Dizziness + …) all run when totals are > 0 — the switch only routes this invocation, it does not mean “one effect globally.”
    // Inside a single case: use separate if blocks (no else) if you want stacked sub-layers for one type; if/else is for mutually exclusive tiers only.
    public static void ApplySingle(EffectType effectType, float value)
    {
        if (value <= MinEffectEpsilon)
        {
            return;
        }

        if (EffectsManager.instance == null)
        {
            Debug.LogError("Effects Manager missing: Can't play effects");
        }

        switch (effectType)
        {
            case EffectType.High:
                if (value >= PotionRules.GreenOutThreshold)
                {
                    // TODO: heavy high / green-out VFX
                    EffectsManager.instance.ActiveHigh(value);
                    Debug.Log("Heavy high / green-out VFX");
                }
                else
                {
                    // TODO: mild high VFX scaled by value
                    Debug.Log("Mild high VFX scaled by value: " + value);
                }
                break;

            case EffectType.Dizziness:
                EffectsManager.instance.ActivateDizzy(value);
                Debug.Log("Dizziness value: " + value);
                break;

            case EffectType.Nausea:
                EffectsManager.instance.ActiveNausea(value);
                Debug.Log("Nausea value: " + value);
                break;

            case EffectType.Hallucination:
                EffectsManager.instance.ActivateHallucination(value);
                Debug.Log("Hallucination value: " + value);
                break;

            case EffectType.Lethargy:
                // TODO: desaturate, slow UI
                EffectsManager.instance.ActivateLethargy(value);
                Debug.Log("Lethargy value: " + value);
                break;

            case EffectType.Focus:
                // TODO: sharpen, calm vignette
                EffectsManager.instance.ActivateFocus(value);
                Debug.Log("Focus value: " + value);
                break;

            default:
                Debug.LogWarning($"[PotionEffectVfxHooks] Unhandled effect: {effectType}");
                break;
        }
    }

    // Map cumulative hallucination (and optionally other types) to 0–1 text distortion for minigames.
    public static float ComputeInterviewDistortion(PlayerPotionStats stats)
    {
        if (stats == null)
        {
            return 0f;
        }

        float hall = stats.GetTotal(EffectType.Hallucination);
        float high = stats.GetTotal(EffectType.High);
        // Tunable: stronger totals ⇒ more * masking in InterviewMiniGame.ApplyDistortion
        float raw = hall * 0.05f + high * 0.02f;
        return Mathf.Clamp01(raw);
    }

    // Optional: slower answers when lethargy / high are high.
    public static float ComputeInterviewTimeMultiplier(PlayerPotionStats stats)
    {
        if (stats == null)
        {
            return 1f;
        }

        float leth = stats.GetTotal(EffectType.Lethargy);
        float focus = stats.GetTotal(EffectType.Focus);
        float mult = 1f + leth * 0.05f - focus * 0.03f;
        return Mathf.Clamp(mult, 0.5f, 2f);
    }
}
