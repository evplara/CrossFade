using System;
using UnityEngine;

/*
 * SessionTimer.cs — Tracks how long the player has been alive this run
 *
 * What lives here:
 *   - A stopwatch that starts when the player begins a new session.
 *   - Uses Time.deltaTime so it auto-pauses when PauseManager freezes timeScale.
 *   - Hidden from UI for now, just tracking in the background.
 *
 * Main APIs / usage:
 *   - SessionTimer.Instance.StartSession() to kick it off when a run begins.
 *   - SessionTimer.Instance.ElapsedSeconds to read how long they've been going.
 *   - StopSession() on death, ResetSession() when going back to main menu.
 *   - TimerTick fires every frame if anything needs live updates later.
 */

public class SessionTimer : MonoBehaviour
{
    public static SessionTimer Instance { get; private set; }
    private float maxRoundTime;
    public float MaxRoundTime => maxRoundTime;


    private float elapsedSeconds;
    private bool isRunning;

    public float ElapsedSeconds => elapsedSeconds;
    public bool IsRunning => isRunning;

    // might be useful later for difficulty scaling or a HUD
    public event Action<float> TimerTick;
    public event Action SessionOver;

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

        elapsedSeconds += Time.deltaTime;
        TimerTick?.Invoke(elapsedSeconds);

        //session ends
        if (elapsedSeconds >= maxRoundTime)
        {
            elapsedSeconds = maxRoundTime;
            StopSession();
        }
    }

    // call once when player starts a new run
    public void StartSession()
    {
        elapsedSeconds = 0f;
        isRunning = true;
        Debug.Log("[SessionTimer] Session started.");
    }

    // pause the clock on death / game over — doesn't reset
    public void StopSession()
    {
        SessionOver?.Invoke();
        isRunning = false;
        Debug.Log($"[SessionTimer] Session stopped at {elapsedSeconds:F1}s.");
    }

    // pick up where we left off without resetting
    public void ResumeSession()
    {
        isRunning = true;
        Debug.Log($"[SessionTimer] Session resumed at {elapsedSeconds:F1}s.");
    }

    // full wipe — back to main menu, fresh run, etc.
    public void ResetSession()
    {
        elapsedSeconds = 0f;
        isRunning = false;
    }

    public void SetTime(float time)
    {
        maxRoundTime = time;
    }

    // returns "MM:SS" — handy if we ever show this on screen
    public string GetFormattedTime()
    {
        float remaining = Mathf.Max(0f, maxRoundTime - elapsedSeconds);
        return remaining.ToString("F1");
    }
}
