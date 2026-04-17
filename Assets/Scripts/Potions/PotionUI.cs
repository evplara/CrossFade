using UnityEngine;

/*
 * PotionUI.cs — PotionController partial: UnityEvents for brewing HUD
 *
 * What lives here:
 *   - OnRollButtonClicked, OnPotionSlotClicked, OnMixButtonClicked, OnConsumeButtonClicked.
 *
 * Main APIs / usage:
 *   - Wire these on UI Buttons in the Inspector (pass slot index for OnPotionSlotClicked where supported).
 *   - OnMixButtonClicked (testing): pairs newest roll with last mix result when possible; else last two slots.
 *   - OnConsumeButtonClicked (testing): drinks last shelf potion; selection-based consume is commented below.
 */

namespace CrossFade.Potions
{
    public partial class PotionController
    {
        public void OnRollButtonClicked(PotionRaritySO weights = null)
        {
            if (!TryRollAndStorePotion(weights))
            {
                Debug.Log("Roll failed.");
                return;
            }

            var inv = Inventory;
            var p = inv[inv.Count - 1];
            Debug.Log($"Potion: {p.Name} | Rarity: {p.Rarity}\n{p.FormatEffectsForDebug()}");
        }

        public void OnPotionSlotClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots)
            {
                return;
            }

            if (SelectedIndex == slotIndex)
            {
                Select(-1);
                return;
            }

            Select(slotIndex);
        }

        // Testing: no shelf selection — mixes newest roll with last mix output when that potion is still on shelf; otherwise last two entries.
        public void OnMixButtonClicked()
        {
            if (!TryGetTestMixIndices(out var leftIndex, out var rightIndex))
            {
                Debug.Log("Mix failed: need at least two potions.");
                return;
            }

            if (!TryMixByIndex(leftIndex, rightIndex))
            {
                Debug.Log("Mix failed.");
                return;
            }

            var mixed = GetAt(leftIndex);
            if (mixed != null)
            {
                Debug.Log($"Mixed potion (newest + last mix when applicable): {mixed.Name} | Rarity: {mixed.Rarity}\n{mixed.FormatEffectsForDebug()}");
            }
        }

        // Scene UI path: explicitly mix two selected inventory indices.
        public bool OnMixButtonClicked(int leftIndex, int rightIndex)
        {
            Debug.Log($"[PotionUI] OnMixButtonClicked(left,right) called | left={leftIndex} right={rightIndex} inventoryCount={Inventory.Count}");
            if (!TryMixByIndex(leftIndex, rightIndex))
            {
                Debug.Log("Mix failed.");
                return false;
            }

            var mixed = GetAt(Mathf.Min(leftIndex, rightIndex));
            if (mixed != null)
            {
                Debug.Log($"Mixed potion: {mixed.Name} | Rarity: {mixed.Rarity}\n{mixed.FormatEffectsForDebug()}");
            }

            return true;
        }

        // Testing: drink the last potion on the shelf (no slot selection). Applies stats via ConsumePotionAt → PlayerPotionStats.
        public void OnConsumeButtonClicked()
        {
            var inv = Inventory;
            if (inv.Count == 0)
            {
                Debug.Log("Drink failed: shelf empty.");
                return;
            }

            var index = inv.Count - 1;
            var consumed = ConsumePotionAt(index);
            if (consumed == null)
            {
                Debug.Log("Drink failed.");
                return;
            }

            LogSimulatedDrinkEffects(consumed);
        }

        // Scene UI path: consume a specific potion index (e.g., cup-selected potion).
        public bool OnConsumeButtonClicked(int index)
        {
            var inv = Inventory;
            Debug.Log($"[PotionUI] OnConsumeButtonClicked(index) called | index={index} inventoryCount={inv.Count}");
            if (index < 0 || index >= inv.Count)
            {
                Debug.Log("Drink failed: invalid index.");
                return false;
            }

            var consumed = ConsumePotionAt(index);
            if (consumed == null)
            {
                Debug.Log("Drink failed.");
                return false;
            }

            LogSimulatedDrinkEffects(consumed);
            return true;
        }

        private static void LogSimulatedDrinkEffects(PotionData potion)
        {
            var stats = PlayerPotionStats.Instance;
            var line = $"[Drink / simulate] {potion.Name} | Rarity: {potion.Rarity}\n{potion.FormatEffectsForDebug()}";

            if (stats == null)
            {
                Debug.Log($"{line}\n(PlayerPotionStats not in scene — effects not stored.)");
                return;
            }

            line += "\n--- Player totals after this drink ---";
            foreach (var t in PotionRules.CoreEffects)
            {
                line += $"\n  {t}: {stats.GetTotal(t):F1}";
            }

            Debug.Log(line);
        }
    }
}
