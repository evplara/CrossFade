using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
