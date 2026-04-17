using System;
using UnityEngine;

/*
 * HealthManager.cs — Global health pool that persists across all minigames
 *
 * What lives here:
 *   - Single HP pool for the entire session. Player starts with MaxHealth (10 for now),
 *     loses HP from minigame damage, and never heals.
 *   - When HP hits 0, PlayerDied fires and the run is over.
 *
 * Main APIs / usage:
 *   - HealthManager.Instance.TakeDamage(amount) — any minigame calls this when the player gets hurt.
 *   - HealthManager.Instance.CurrentHealth / MaxHealth — read for HUD.
 *   - HealthChanged fires on any HP change, PlayerDied fires once at 0.
 *   - ResetHealth() at the start of a new run.
 *
 * Plugging into a new minigame:
 *   Just call HealthManager.Instance.TakeDamage(1) whenever the player gets hit.
 *   The manager handles everything else — events, clamping, death.
 */

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    //10 for now, might change later
    [SerializeField] private int maxHealth = 10;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // fires whenever HP changes — passes (currentHealth, maxHealth) for HUD stuff
    public event Action<int, int> HealthChanged;

    // fires once when HP hits 0
    public event Action PlayerDied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentHealth = maxHealth;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // call this from any minigame when the player gets hurt
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; // already dead
        if (amount <= 0) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"[HealthManager] Took {amount} damage. HP: {currentHealth}/{maxHealth}");

        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("[HealthManager] Player died.");
            PlayerDied?.Invoke();
        }
    }

    // call when starting a fresh run
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
