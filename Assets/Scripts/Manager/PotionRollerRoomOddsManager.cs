using UnityEngine;

public class PotionRollerRoomOddsManager : MonoBehaviour
{
    public static PotionRollerRoomOddsManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }
}
