using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Multi-scene manager:
// Main Menu, Credits, Guide, Pause Menu, and Game Over
// Potion Room
// minigames include Interview, Cooking, and Car MiniGame
public class HandleSceneManager : MonoBehaviour
{
    public static HandleSceneManager instance;

    [SerializeField] private string[] miniGameSceneNames;
    private string currentSceneName;

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
        LoadScene("SlotMachine");
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("HandleSceneManager.LoadScene: empty scene name.");
            return;
        }

        SceneManager.LoadScene(sceneName);
        currentSceneName = sceneName;
    }

    public void LoadRandomMiniGameScene()
    {
        if (miniGameSceneNames == null || miniGameSceneNames.Length == 0)
        {
            //default backup
            LoadPotionScene();
            return;
        }

        var filterScenes = miniGameSceneNames.Where(scene => scene != currentSceneName).ToArray();

        // if every scene got filtered out (only one minigame and we're in it), go back to potions
        if (filterScenes.Length == 0)
        {
            LoadPotionScene();
            return;
        }

        int random = Random.Range(0, filterScenes.Length);
        string targetScene = filterScenes[random];

        LoadScene(targetScene);
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
