using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    // ── Stats ────────────────────────────────────────────────────

    public float Health { get; private set; }
    public float Money { get; private set; }
    public float TotalAliveTime { get; private set; }

    // ── Config ───────────────────────────────────────────────────

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float startingMoney = 0f;

    // ── Unity lifecycle ──────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Initialize()
    {
        Health = maxHealth;
        Money = startingMoney;
        TotalAliveTime = 0f;
    }

    // ── Health ───────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        // TODO: implement
    }

    public void Heal(float amount)
    {
        // TODO: implement
    }

    // ── Money ────────────────────────────────────────────────────

    public void EarnMoney(float amount)
    {
        // TODO: implement
    }

    public bool SpendMoney(float amount)
    {
        // TODO: implement (return false if insufficient funds)
        return false;
    }

    // ── Time ─────────────────────────────────────────────────────

    public void AddAliveTime(float seconds)
    {
        // TODO: implement (called by MinigameTimer on session end)
    }
}
