using System;

using System.Collections.Generic;

using UnityEngine;



namespace CrossFade.Potions

{

    // Responsible for rarity rolls, template selection, and effect value rolling.

    public class PotionRoller

    {

        private static readonly PotionRarity[] RarityOrder =

        {

            PotionRarity.Common,

            PotionRarity.Uncommon,

            PotionRarity.Rare,

            PotionRarity.Epic,

            PotionRarity.Legendary

        };



        private readonly Dictionary<PotionRarity, float> _rarityWeights = new();





        // Initializes default rarity weights and any baseline roller settings.



        public PotionRoller()

        {

            // 1) Populate _rarityWeights for each rarity tier.

            // 2) Optionally validate all required tiers exist.

            foreach (var r in RarityOrder)

            {

                _rarityWeights[r] = r switch

                {

                    PotionRarity.Common => 55f,

                    PotionRarity.Uncommon => 25f,

                    PotionRarity.Rare => 12f,

                    PotionRarity.Epic => 6f,

                    PotionRarity.Legendary => 2f,

                    _ => 1f

                };

            }

        }





        // Rolls a complete potion instance from available templates.



        // templates: All possible templates eligible for rolling.

        // Returns: Newly generated potion instance.

        public PotionData RollPotion(IReadOnlyList<PotionTemplate> templates)

        {

            // 1) Roll rarity tier with weighted probabilities.

            // 2) Filter templates by rolled rarity.

            // 3) Pick one template at random.

            // 4) Build PotionData using rolled effects from template ranges.

            // 5) Return created potion instance.

            if (templates == null)

            {

                throw new ArgumentNullException(nameof(templates));

            }

            if (templates.Count == 0)

            {

                throw new ArgumentException("Templates cannot be empty.", nameof(templates));

            }



            var rarity = RollRarity();

            var matches = new List<PotionTemplate>();

            for (var i = 0; i < templates.Count; i++)

            {

                var t = templates[i];

                if (t != null && t.Rarity == rarity)

                {

                    matches.Add(t);

                }

            }

            if (matches.Count == 0)

            {

                throw new ArgumentException("No template matches rolled rarity: " + rarity, nameof(templates));

            }

            var pick = matches[UnityEngine.Random.Range(0, matches.Count)];

            return BuildPotionFromTemplate(pick);

        }





        // Rolls a rarity tier based on configured weights.



        // Returns: Chosen rarity value.

        public PotionRarity RollRarity()

        {

            // 1) Sum all rarity weights.

            // 2) Generate random value from [0, totalWeight).

            // 3) Walk cumulative weights to find selected rarity.

            if (_rarityWeights.Count == 0)

            {

                throw new InvalidOperationException("Rarity weights are empty.");

            }



            var totalWeight = 0f;

            for (var i = 0; i < RarityOrder.Length; i++)

            {

                var r = RarityOrder[i];

                if (_rarityWeights.TryGetValue(r, out var w) && w > 0f)

                {

                    totalWeight += w;

                }

            }

            if (totalWeight <= 0f)

            {

                throw new InvalidOperationException("Total rarity weight must be positive.");

            }



            var roll = UnityEngine.Random.Range(0f, totalWeight);

            var cumulative = 0f;

            for (var i = 0; i < RarityOrder.Length; i++)

            {

                var r = RarityOrder[i];

                if (!_rarityWeights.TryGetValue(r, out var w) || w <= 0f)

                {

                    continue;

                }

                cumulative += w;

                if (roll <= cumulative)

                {

                    return r;

                }

            }

            return RarityOrder[RarityOrder.Length - 1];

        }





        // Creates a runtime potion instance from a selected template.

        // Maybe from CSV file?



        // template: Template used as source data.

        // Returns: Runtime potion data instance.

        public PotionData BuildPotionFromTemplate(PotionTemplate template)

        {

            // 1) Create new PotionData and copy static fields.

            // 2) Roll each effect value inside template min/max range.

            // 3) Append rolled effects to PotionData.Effects.

            if (template == null)

            {

                throw new ArgumentNullException(nameof(template));

            }

            var potion = new PotionData

            {

                Name = template.DisplayName,

                Rarity = template.Rarity

            };

            var ranges = template.AvailableEffects;

            if (ranges == null)

            {

                return potion;

            }

            for (var i = 0; i < ranges.Count; i++)

            {

                var effect = ranges[i];

                var rolledValue = RollEffectValue(effect);

                potion.Effects.Add(new PotionEffectValue(effect.EffectType, rolledValue));

            }

            return potion;

        }





        // Sets or overrides rarity weights at runtime for balancing/testing.



        // newWeights: Dictionary of rarity to weight.

        public void SetRarityWeights(Dictionary<PotionRarity, float> newWeights)

        {

            // 1) Validate dictionary includes expected keys and positive values.

            // 2) Replace _rarityWeights with validated values.

            if (newWeights == null)

            {

                throw new ArgumentNullException(nameof(newWeights));

            }

            if (newWeights.Count == 0)

            {

                throw new ArgumentException("Weights cannot be empty.", nameof(newWeights));

            }

            _rarityWeights.Clear();

            foreach (var kvp in newWeights)

            {

                _rarityWeights[kvp.Key] = kvp.Value;

            }

        }



        private static float RollEffectValue(EffectRange effect)

        {

            if (effect == null)

            {

                throw new ArgumentNullException(nameof(effect));

            }

            var min = Mathf.Min(effect.MinValue, effect.MaxValue);

            var max = Mathf.Max(effect.MinValue, effect.MaxValue);

            return UnityEngine.Random.Range(min, max);

        }

    }

}


