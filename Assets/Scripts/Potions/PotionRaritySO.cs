using UnityEngine;
using CrossFade.Potions;

[CreateAssetMenu(menuName = "Scriptable Objects/Potion Rarity")]
public class PotionRaritySO : ScriptableObject
{
    [Tooltip("Inclusive min for each core effect roll (e.g. 1 standard, 10 improved, 20 best).")]
    public int effectRollMin = PotionRules.MinRollValue;

    [Tooltip("Inclusive max for each core effect roll (e.g. 10 standard, 20 improved, 40 best).")]
    public int effectRollMax = PotionRules.MaxRollValue;

    public PotionRarityWeight[] rarityValues = new PotionRarityWeight[5];
}

[System.Serializable]
public class PotionRarityWeight
{
    public PotionRarity rarity;
    public float value;
}
