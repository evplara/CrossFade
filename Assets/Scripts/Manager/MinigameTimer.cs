using UnityEngine;

public class MinigameTimer : MonoBehaviour
{
    // ── State ────────────────────────────────────────────────────

    public float ElapsedTime { get; private set; }
    public float TimeLimit { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsExpired => TimeLimit > 0f && ElapsedTime >= TimeLimit;

    // ── Unity lifecycle ──────────────────────────────────────────

    private void Update()
    {
        // TODO: implement tick
    }

    // ── Controls ─────────────────────────────────────────────────

    /// <summary>
    /// Start a countdown timer. Pass 0 for no limit (count up only).
    /// </summary>
    public void StartTimer(float timeLimit = 0f)
    {
        // TODO: implement
    }

    public void StopTimer()
    {
        // TODO: implement
    }

    public void ResetTimer()
    {
        // TODO: implement
    }

    /// <summary>
    /// Call this when the minigame ends to bank elapsed time into PlayerStats.
    /// </summary>
    public void CommitTimeToPlayerStats()
    {
        // TODO: implement — calls PlayerStats.Instance.AddAliveTime(ElapsedTime)
    }
}
