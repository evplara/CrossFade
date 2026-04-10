using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    // Populate these in the Inspector with one entry per stat
    [SerializeField] private List<StatDefinition> statDefinitions = new List<StatDefinition>();

    private readonly List<StatEffect> _activeEffects = new List<StatEffect>();

    // Unity lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Session lifecycle

    /// Apply a potion's effects for the upcoming minigame session.
    /// Call this in the hub before entering a minigame.
    public void ApplyEffect(StatEffect effect)
    {
        _activeEffects.Add(effect);
    }

    /// Remove all active effects. Call this when returning to the hub.
    public void ClearAllEffects()
    {
        _activeEffects.Clear();
    }

    /// Convenience method to hook into your minigame end event.
    public void OnMinigameEnd()
    {
        ClearAllEffects();
    }

    // Stat queries

    /// Returns the current value of a Numeric stat (base + all flat deltas stacked).
    public float GetNumeric(StatId id)
    {
        StatDefinition def = GetDefinition(id);

        if (def == null || def.type != StatType.Numeric)
        {
            Debug.LogWarning($"[StatsManager] '{id}' is not a registered Numeric stat.");
            return 0f;
        }

        float value = def.baseValue;

        foreach (StatEffect effect in _activeEffects)
        {
            foreach (StatModifier mod in effect.modifiers)
            {
                if (mod.target == id && mod.modifierType == ModifierType.Numeric)
                {
                    value += mod.flatDelta;
                }
            }
        }

        return value;
    }

    /// Returns the current value of a Bool stat.
    /// Any active modifier that sets it true wins (OR logic).
    public bool GetBool(StatId id)
    {
        StatDefinition def = GetDefinition(id);

        if (def == null || def.type != StatType.Bool)
        {
            Debug.LogWarning($"[StatsManager] '{id}' is not a registered Bool stat.");
            return false;
        }

        foreach (StatEffect effect in _activeEffects)
        {
            foreach (StatModifier mod in effect.modifiers)
            {
                if (mod.target == id && mod.modifierType == ModifierType.Bool && mod.boolValue)
                {
                    return true;
                }
            }
        }

        return def.defaultBool;
    }

    // Helpers

    private StatDefinition GetDefinition(StatId id)
    {
        return statDefinitions.Find(d => d.id == id);
    }

    /// Logs all current resolved stat values. Useful during development.
    public void DebugPrintAll()
    {
        Debug.Log("[StatsManager] ── Current stat values ──");
        foreach (StatDefinition def in statDefinitions)
        {
            if (def.type == StatType.Numeric)
                Debug.Log($"  {def.id} (Numeric): {GetNumeric(def.id)}");
            else
                Debug.Log($"  {def.id} (Bool): {GetBool(def.id)}");
        }
    }
}
