using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// Multi-scene manager: 
// Main Menu, Credits, Guide, Pause Menu, and Game Over
// Potion Room
// minigames include Interview, Cooking, and Car MiniGame
public class HandleSceneManager : MonoBehaviour
{
    public static HandleSceneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(this);
    }

    //maybe can switch to enums or specific value if needed
    public void LoadScene(string targetScene)
    {
        SceneManager.LoadSceneAsync(targetScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
