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
 */

namespace CrossFade.Potions
{
    public partial class PotionController
    {
        public void OnRollButtonClicked()
        {
            if (!TryRollAndStorePotion())
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

        /*
         * Selection-based mix (restore when shelf UI exists):
         *
         * public void OnMixButtonClicked()
         * {
         *     var inv = Inventory;
         *     if (inv.Count < 2)
         *     {
         *         Debug.Log("Mix failed: need at least two potions.");
         *         return;
         *     }
         *
         *     var lastRolledIndex = inv.Count - 1;
         *     if (SelectedIndex < 0)
         *     {
         *         Debug.Log("Mix failed: select a shelf potion to mix with the current roll.");
         *         return;
         *     }
         *
         *     if (SelectedIndex == lastRolledIndex)
         *     {
         *         Debug.Log("Mix failed: pick a different slot than the current roll.");
         *         return;
         *     }
         *
         *     if (!TryMixByIndex(SelectedIndex, lastRolledIndex))
         *     {
         *         Debug.Log("Mix failed.");
         *         return;
         *     }
         *
         *     var mixed = GetAt(SelectedIndex);
         *     if (mixed != null)
         *     {
         *         Debug.Log($"Mixed potion: {mixed.Name} | Rarity: {mixed.Rarity}");
         *     }
         * }
         */

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

        public void OnConsumeButtonClicked()
        {
            if (SelectedIndex < 0)
            {
                Debug.Log("Consume failed: no potion selected.");
                return;
            }

            var consumed = ConsumePotionAt(SelectedIndex);
            if (consumed == null)
            {
                Debug.Log("Consume failed.");
                return;
            }

            Debug.Log($"Consumed potion: {consumed.Name} | Rarity: {consumed.Rarity}");
        }
    }
}
