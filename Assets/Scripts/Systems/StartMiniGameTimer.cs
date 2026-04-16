using UnityEngine;
using CrossFade.Potions;

public class StartMiniGameTimer : MonoBehaviour
{
    private void Start()
    {
        MinigameTimer.Instance.StartTimerValue();
    }
}
