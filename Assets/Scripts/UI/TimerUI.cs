using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundTimerText;
    [SerializeField] private TextMeshProUGUI miniGameTimerText;


    private void Update()
    {
        if (MinigameTimer.Instance != null && SessionTimer.Instance != null)
        {
            roundTimerText.text = SessionTimer.Instance.GetFormattedTime();
            miniGameTimerText.text = MinigameTimer.Instance.GetFormattedTime();
        }
    }
}
