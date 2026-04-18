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

    // Prevent stale singleton references when Enter Play Mode Options disables domain reload.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticStateOnPlay()
    {
        instance = null;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            var oldInstance = instance;
            if ((miniGameSceneNames == null || miniGameSceneNames.Length == 0)
                && oldInstance.miniGameSceneNames != null
                && oldInstance.miniGameSceneNames.Length > 0)
            {
                miniGameSceneNames = oldInstance.miniGameSceneNames;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            if (oldInstance != null)
            {
                Destroy(oldInstance.gameObject);
            }

            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
        if (instance == this)
        {
            instance = null;
        }
    }

    public void LoadGameOverScene()
    {
        LoadScene("GameOverScreen");
    }

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
        currentSceneName = SceneManager.GetActiveScene().name;
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

    private void HandleActiveSceneChanged(Scene previous, Scene next)
    {
        currentSceneName = next.name;
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
