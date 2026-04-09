using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    //maybe these will be useful for stuff like the pausemenu
    public event Action GamePaused;
    public event Action GameUnpaused;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GamePaused?.Invoke();
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        GameUnpaused?.Invoke();
    }


}
