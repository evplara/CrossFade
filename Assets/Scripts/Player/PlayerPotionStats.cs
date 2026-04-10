using System;
using CrossFade.Potions;
using UnityEngine;

/*
 * PlayerPotionStats.cs — Player-side totals from consumed potions (PotionData effect values)
 *
 * What lives here:
 *   - Float totals per EffectType (sized from PotionRules.CoreEffects, same catalog as PotionData).
 *   - ApplyConsumedPotion: adds GetEffectValue per type; ResetTotals clears the session.
 *
 * Main APIs / usage:
 *   - Add component to Player (or persistent root); PotionController calls ApplyConsumedPotion on consume.
 *   - HUD / minigames read GetTotal(EffectType) and subscribe to StatsChanged.
 */

public class PlayerPotionStats : MonoBehaviour
{
    public static PlayerPotionStats Instance { get; private set; }

    // One slot per EffectType ordinal; sized from PotionRules.CoreEffects (same catalog as PotionData).
    private readonly float[] _totals = new float[PotionRules.CoreEffects.Length];

    public event Action StatsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public float GetTotal(EffectType effectType)
    {
        var i = (int)effectType;
        if (i < 0 || i >= _totals.Length)
        {
            return 0f;
        }

        return _totals[i];
    }

    public void ResetTotals()
    {
        Array.Clear(_totals, 0, _totals.Length);
        StatsChanged?.Invoke();
    }

    // Invoked by PotionController when a potion is consumed from inventory.
    public void ApplyConsumedPotion(PotionData potion)
    {
        if (potion == null)
        {
            return;
        }

        foreach (var effectType in PotionRules.CoreEffects)
        {
            AddToTotal(effectType, potion.GetEffectValue(effectType));
        }

        StatsChanged?.Invoke();
    }

    private void AddToTotal(EffectType effectType, float delta)
    {
        var idx = (int)effectType;
        if (idx < 0 || idx >= _totals.Length)
        {
            return;
        }

        _totals[idx] += delta;
    }
}
