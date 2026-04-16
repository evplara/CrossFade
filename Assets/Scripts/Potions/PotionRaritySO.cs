using UnityEngine;
using CrossFade.Potions;

[CreateAssetMenu(menuName = "Scriptable Objects/Potion Rarity")]
public class PotionRaritySO : ScriptableObject
{
    public PotionRarityWeight[] rarityValues = new PotionRarityWeight[5];
}

[System.Serializable]
public class PotionRarityWeight
{
    public PotionRarity rarity;
    public float value;
}
