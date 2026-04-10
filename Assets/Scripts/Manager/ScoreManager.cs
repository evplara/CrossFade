using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // ── State ────────────────────────────────────────────────────

    public float FinalScore { get; private set; }

    // ── Weights (tweak in Inspector) ─────────────────────────────

    [SerializeField] private float aliveTimeWeight = 1f;
    [SerializeField] private float moneyWeight = 1f;

    // ── Unity lifecycle ──────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Score calculation ────────────────────────────────────────

    /// <summary>
    /// Call this on game over. Pulls data from PlayerStats and computes the final score.
    /// </summary>
    public void CalculateFinalScore()
    {
        // TODO: implement
        // FinalScore = (PlayerStats.Instance.TotalAliveTime * aliveTimeWeight)
        //            + (PlayerStats.Instance.Money * moneyWeight);
    }

    /// <summary>
    /// Add more scoring factors here as the game grows.
    /// </summary>
    private float GetBonuses()
    {
        // TODO: implement
        return 0f;
    }
}
