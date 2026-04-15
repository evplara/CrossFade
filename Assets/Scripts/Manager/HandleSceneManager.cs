using UnityEngine;
using UnityEngine.SceneManagement;

// Multi-scene manager:
// Main Menu, Credits, Guide, Pause Menu, and Game Over
// Potion Room
// minigames include Interview, Cooking, and Car MiniGame
public class HandleSceneManager : MonoBehaviour
{
    public static HandleSceneManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    //should change this later...
    public void LoadPotionScene()
    {
        SceneManager.LoadSceneAsync("PotionRoom2");
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("HandleSceneManager.LoadScene: empty scene name.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
