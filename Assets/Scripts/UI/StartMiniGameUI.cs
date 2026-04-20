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
        if (PotionController.Instance != null)
        {
            PotionController.Instance.OnPotionConsumed -= UpdatePotions;
        }
    }

    private void UpdatePotions(PotionData data)
    {
        potionsConsumed.Add(data);
    }

    public void StartSessionTimer()
    {
        float gameValue = PotionTiming.ResolveSecondsPerGame(potionsConsumed);
        float roundValue = PotionTiming.ResolveRoundTotalSeconds(potionsConsumed);
        float highestValue = PotionTiming.GetEffectValue(potionsConsumed);

        //greened out case
        if (highestValue == -1) highestValue = PlayerPotionStats.Instance.MaxEffectTotal;

        SessionManager.Instance.SetHighScore(highestValue);
        SessionTimer.Instance.SetTime(roundValue);
        MinigameTimer.Instance.SetTime(gameValue);

        SessionTimer.Instance.StartSession();
        HandleSceneManager.instance.LoadRandomMiniGameScene();
    }
}
