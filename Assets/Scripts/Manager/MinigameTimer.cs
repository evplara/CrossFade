using System;
using UnityEngine;

/*
 * MinigameTimer.cs — Countdown timer for each minigame round
 *
 * What lives here:
 *   - A countdown that starts when a minigame scene loads.
 *   - When it hits 0, fires TimerExpired — the minigame listens to that
 *     and handles its own exit (stop spawning cars, auto-submit answers, etc.).
 *   - Duration values come from PotionTiming.ResolveSecondsPerGame().
 *   - Uses Time.deltaTime so it respects PauseManager.
 *
 * Main APIs / usage:
 *   - MinigameTimer.Instance.StartTimer(seconds) to kick off a countdown.
 *   - MinigameTimer.Instance.StopTimer() for early stop (doesn't fire expired).
 *   - RemainingSeconds / NormalizedRemaining for HUD stuff.
 *   - TimerTick fires every frame, TimerExpired fires once at 0.
 *
 * How a minigame hooks into this:
 *   void Start() {
 *       MinigameTimer.Instance.TimerExpired += OnTimeUp;
 *       MinigameTimer.Instance.StartTimer(PotionTiming.ResolveSecondsPerGame(potion));
 *   }
 *   void OnDestroy() {
 *       if (MinigameTimer.Instance != null)
 *           MinigameTimer.Instance.TimerExpired -= OnTimeUp;
 *   }
 */

public class MinigameTimer : MonoBehaviour
{
    public static MinigameTimer Instance { get; private set; }
    private float secondsPerRound;

    private float remainingSeconds;
    private float totalSeconds;
    private bool isRunning;

    public float RemainingSeconds => remainingSeconds;
    public float TotalSeconds => totalSeconds;
    public bool IsRunning => isRunning;

    // goes from 1 (full) to 0 (expired) — useful for UI fill bars
    public float NormalizedRemaining =>
        totalSeconds > 0f ? Mathf.Clamp01(remainingSeconds / totalSeconds) : 0f;

    // fires every frame with remaining seconds — hook up HUD text here
    public event Action<float> TimerTick;

    // fires once when countdown hits 0
    public event Action TimerExpired;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingSeconds -= Time.deltaTime;

        if (remainingSeconds <= 0f)
        {
            remainingSeconds = 0f;
            isRunning = false;

            Debug.Log("[MinigameTimer] Timer expired.");
            TimerTick?.Invoke(0f);
            TimerExpired?.Invoke();
            HandleSceneManager.instance.LoadRandomMiniGameScene();
        }
        else
        {
            TimerTick?.Invoke(remainingSeconds);
        }
    }

    // starts a new countdown — replaces any existing one
    public void StartTimer(float seconds)
    {
        totalSeconds = seconds;
        remainingSeconds = seconds;
        isRunning = true;
        Debug.Log($"[MinigameTimer] Started countdown: {seconds:F1}s");
    }

    public void StartTimerValue()
    {
        totalSeconds = secondsPerRound;
        remainingSeconds = secondsPerRound;
        isRunning = true;
    }

    public void SetTime(float time)
    {
        secondsPerRound = time;
    }

    // stop early without firing TimerExpired (player finished before time ran out, etc.)
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"[MinigameTimer] Stopped with {remainingSeconds:F1}s remaining.");
    }

    // zero everything out between minigame transitions
    public void ResetTimer()
    {
        remainingSeconds = 0f;
        totalSeconds = 0f;
        isRunning = false;
    }

    // "12.3" for short timers, "1:05" for longer ones — for HUD display
    public string GetFormattedTime()
    {
        if (totalSeconds < 60f)
        {
            return $"{remainingSeconds:F1}";
        }

        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
        return $"{minutes}:{seconds:00}";
    }
}
