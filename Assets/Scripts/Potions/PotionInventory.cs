using System;

/*
 * PotionInventory.cs — PotionController partial: shelf selection and inventory reads
 *
 * What lives here:
 *   - Select, GetAt, ClearSlotCache; RefreshCache and slot cache aligned with Inventory.
 *
 * Main APIs / usage:
 *   - Called by PotionUI and other UI; same partial class as PotionController (shared state).
 */

namespace CrossFade.Potions
{
    public partial class PotionController
    {
        public void Select(int index)
        {
            if (index < -1 || index >= _maxSlots)
            {
                return;
            }

            if (index >= 0)
            {
                var atSlot = GetAt(index);
                if (atSlot == null)
                {
                    return;
                }

                _cachedPotionBySlot[index] = atSlot;
            }

            SelectedIndex = index;
            RefreshCache();
            SelectionChanged?.Invoke(SelectedIndex);
        }

        private void RefreshCache()
        {
            Array.Clear(_cachedPotionBySlot, 0, _cachedPotionBySlot.Length);

            for (var i = 0; i < _inventory.Count && i < _cachedPotionBySlot.Length; i++)
            {
                _cachedPotionBySlot[i] = _inventory[i];
            }
        }

        private PotionData GetAtInternal(int index)
        {
            if (index < 0 || index >= _inventory.Count)
            {
                return null;
            }

            return _inventory[index];
        }

        public PotionData GetAt(int index)
        {
            if (index < 0 || index >= _maxSlots)
            {
                return null;
            }

            var fromInventory = GetAtInternal(index);
            if (fromInventory != null)
            {
                return fromInventory;
            }

            return index < _cachedPotionBySlot.Length ? _cachedPotionBySlot[index] : null;
        }

        public void ClearSlotCache(int index)
        {
            if (index >= 0 && index < _cachedPotionBySlot.Length)
            {
                _cachedPotionBySlot[index] = null;
            }

            if (SelectedIndex == index)
            {
                Select(-1);
            }
        }
    }
}
