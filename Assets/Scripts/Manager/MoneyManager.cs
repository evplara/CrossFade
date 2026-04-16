using System;
using UnityEngine;

/*
 * MoneyManager.cs — Global money tracker that persists across scenes
 *   - The player's money balance for the session.
 *   - ChangeMoney(int) to add or subtract. Balance can't go below 0.
 *   - MoneyChanged event for UI and button interactability.
 *
 *   - MoneyManager.Instance.ChangeMoney(amount) — minigames reward, potion shop deducts.
 *   - MoneyManager.Instance.CurrentMoney — read balance.
 *   - Subscribe to MoneyChanged for HUD updates.
 *   - ResetMoney() when starting a fresh run.
 */

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private int currentMoney;

    public int CurrentMoney => currentMoney;

    // fires whenever balance changes — hook up HUD, buy buttons, etc.
    public event Action MoneyChanged;

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

    // positive = earn, negative = spend. won't go below 0.
    public void ChangeMoney(int amount)
    {
        currentMoney = Mathf.Max(currentMoney + amount, 0);
        Debug.Log($"[MoneyManager] Money changed by {amount}. Balance: {currentMoney}");
        MoneyChanged?.Invoke();
    }

    // call when starting a fresh run
    public void ResetMoney()
    {
        currentMoney = 0;
        MoneyChanged?.Invoke();
    }
}
