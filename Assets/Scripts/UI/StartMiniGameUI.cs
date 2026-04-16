using UnityEngine;
using CrossFade.Potions;
using System.Collections.Generic;
public class StartMiniGameUI : MonoBehaviour
{
    private List<PotionData> potionsConsumed = new(); 

    private void OnEnable()
    {
        PotionController.Instance.OnPotionConsumed += UpdatePotions;
    }
    private void OnDisable()
    {
        PotionController.Instance.OnPotionConsumed -= UpdatePotions;
    }

    private void UpdatePotions(PotionData data)
    {
        potionsConsumed.Add(data);
    }

    public void StartSessionTimer()
    {
        float gameValue = PotionTiming.ResolveSecondsPerGame(potionsConsumed);
        float roundValue = PotionTiming.ResolveRoundTotalSeconds(potionsConsumed);

        SessionTimer.Instance.SetTime(roundValue);
        MinigameTimer.Instance.SetTime(gameValue);

        SessionTimer.Instance.StartSession();
        HandleSceneManager.instance.LoadRandomMiniGameScene();
    }
}
