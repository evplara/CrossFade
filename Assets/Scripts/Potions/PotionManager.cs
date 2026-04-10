using System;
using System.Collections.Generic;

/*
 * PotionManager.cs — Inventory orchestration for potions
 *
 * What it does:
 *   Holds a list of PotionData, template list for rolling, and delegates generation to PotionRoller
 *   and combination to PotionMixer.
 *
 * Members / usage:
 *   - Constructor(PotionRoller, PotionMixer, maxSlots): wire dependencies and capacity.
 *   - SetTemplates: load/replace PotionTemplate list before rolls (required for RollPotion).
 *   - TryRollAndStorePotion: if slot free, calls _roller.RollPotion(_templates) and appends to inventory.
 *   - TryMixByIndex: combines two slots via _mixer.Mix, replaces lower index, removes higher index.
 *   - ConsumePotion: marks consumed, removes from inventory, returns PotionData for applying stats (see TODO hooks).
 *   - HasFreeSlot / Inventory: capacity and read-only list for UI or game systems.
 */

namespace CrossFade.Potions
{
    // Coordinates potion inventory flow: rolling, storing, mixing, and consuming.
    public class PotionManager
    {
        private readonly PotionRoller _roller;
        private readonly PotionMixer _mixer;
        private readonly List<PotionData> _inventory = new();
        private readonly List<PotionTemplate> _templates = new();


        // Maximum shelf/slot capacity in the potion room.
        public int MaxSlots { get; private set; }


        // Returns: Read-only snapshot of current inventory potions.
        public IReadOnlyList<PotionData> Inventory => _inventory;


        // Creates manager with dependencies and initial slot capacity.
        public PotionManager(PotionRoller roller, PotionMixer mixer, int maxSlots)
        {
            // Validation and null checks
            if (roller == null)
            {
                throw new ArgumentNullException(nameof(roller));
            }
            if (mixer == null)
            {
                throw new ArgumentNullException(nameof(mixer));
            }
            if (maxSlots <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlots));
            }

            // Initialize collections
            _roller = roller;       // Roller dependency
            _mixer = mixer;         // Mixer dependency
            MaxSlots = maxSlots;    // Inventory slot limit
        }

        // Loads template data used for future potion rolls.
        public void SetTemplates(List<PotionTemplate> templates)
        {
            // Validate incoming list is not null/empty.
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }
            if (templates.Count == 0)
            {
                throw new ArgumentException("Templates cannot be empty.", nameof(templates));
            }
            
            // Replace manager template cache
            _templates.Clear();
            _templates.AddRange(templates);
        }

        // Rolls a new potion and adds it to inventory if space exists.
        public bool TryRollAndStorePotion()
        {
            if (!HasFreeSlot())
            {
                return false;
            }

            // Requests new potion from _roller and adds to _inventory
            var potion = _roller.RollPotion(_templates);
            _inventory.Add(potion);
            return true;
        }

        // Attempts to mix two inventory potions and store mixed result.
        public bool TryMixByIndex(int leftIndex, int rightIndex)
        {
            // Validate indices and distinct selection.
            if (leftIndex < 0 || leftIndex >= _inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(leftIndex));
            }
            if (rightIndex < 0 || rightIndex >= _inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rightIndex));
            }
            if (leftIndex == rightIndex)
            {
                throw new ArgumentException("Indices must differ.");
            }

            // Remove source potions and store mixture output
            var leftPotion = _inventory[leftIndex];
            var rightPotion = _inventory[rightIndex];
            var mixedPotion = _mixer.Mix(leftPotion, rightPotion);
            var higherIndex = Math.Max(leftIndex, rightIndex);
            var lowerIndex = Math.Min(leftIndex, rightIndex);
            _inventory[lowerIndex] = mixedPotion;
            _inventory.RemoveAt(higherIndex);
            return true;
        }

        // Marks a potion as consumed and removes it from inventory.
        public PotionData ConsumePotion(int index)
        {
            // Validate index and fetch potion.
            if (index < 0 || index >= _inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            var potion = _inventory[index];
            potion.IsConsumed = true;   // Set property
            _inventory.RemoveAt(index); // Remove potion

            // TODO(teammate): Apply potion effect totals to shared PlayerStats and timer systems here.
            // TODO(teammate): Money/health runtime systems should read those shared stats, not inventory state.

            return potion; // Potion that was consumed
        }

        // Returns: turns true when the inventory has available space.
        public bool HasFreeSlot()
        {
            return _inventory.Count < MaxSlots;
        }
    }
}
