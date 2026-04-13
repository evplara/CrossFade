using UnityEngine;

//Setup Car Minigame
public class CarSetup : MonoBehaviour
{
    private void Start()
    {
        ApplyPotionContextFromSession();
    }

    void ApplyPotionContextFromSession()
    {
        PotionEffectVfxHooks.ApplyAllFromPlayerStats();

        var stats = PlayerPotionStats.Instance;
        if (stats != null)
        {
            //use this for later?
            float timeMul = PotionEffectVfxHooks.ComputeInterviewTimeMultiplier(stats);
        }
    }
}
