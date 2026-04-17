using UnityEngine;
using UnityEngine.SceneManagement;

public class ClosePanel : MonoBehaviour
{
    public GameObject panel;
    public string sceneName;

    public void Close()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("don't change scene yet");
        }
        panel.SetActive(false);
    }
}