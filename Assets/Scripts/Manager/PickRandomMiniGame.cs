using UnityEngine;

public class PickRandomMiniGame : MonoBehaviour
{
    [SerializeField] private string[] miniGameNames;

    public void RandomMiniGame()
    {
        int random = Random.Range(0, miniGameNames.Length);

        HandleSceneManager.instance.LoadScene(miniGameNames[random]);
    }
}
