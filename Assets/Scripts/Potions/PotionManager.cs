using System;
using System.Collections.Generic;

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

        // roller: Potion roller dependency.
        // mixer: Potion mixer dependency.
        // maxSlots: Inventory slot limit.
        public PotionManager(PotionRoller roller, PotionMixer mixer, int maxSlots)
        {
            // 1) Assign dependencies.
            // 2) Validate and store max slot count.
            // 3) Initialize collections as needed.
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
            _roller = roller;
            _mixer = mixer;
            MaxSlots = maxSlots;
        }


        // Loads template data used for future potion rolls.

        // templates: Template set to keep in manager state.
        public void SetTemplates(List<PotionTemplate> templates)
        {
            // 1) Validate incoming list is not null/empty.
            // 2) Replace manager template cache.
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }
            if (templates.Count == 0)
            {
                throw new ArgumentException("Templates cannot be empty.", nameof(templates));
            }
            _templates.Clear();
            _templates.AddRange(templates);
        }


        // Rolls a new potion and adds it to inventory if space exists.

        // Returns: True when potion is successfully added.
        public bool TryRollAndStorePotion()
        {
            // 1) Check if inventory has free slot.
            // 2) Request new potion from _roller.
            // 3) Add potion to _inventory.
            // 4) Return success/failure state.
            if (!HasFreeSlot())
            {
                return false;
            }
            var potion = _roller.RollPotion(_templates);
            _inventory.Add(potion);
            return true;
        }


        // Attempts to mix two inventory potions and store mixed result.

        // leftIndex: First potion inventory index.
        // rightIndex: Second potion inventory index.
        // Returns: True when mix succeeds and inventory is updated.
        public bool TryMixByIndex(int leftIndex, int rightIndex)
        {
            // 1) Validate indices and distinct selection.
            // 2) Read source potions.
            // 3) Call _mixer.Mix and handle failure.
            // 4) Remove source potions and store mixed output.
            // 5) Return success/failure.
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

        // index: Inventory index to consume.
        // Returns: Potion that was consumed.
        public PotionData ConsumePotion(int index)
        {
            // 1) Validate index and fetch potion.
            // 2) Mark potion IsConsumed = true.
            // 3) Remove potion from inventory.
            // 4) Return consumed potion for external stat application.
            if (index < 0 || index >= _inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            var potion = _inventory[index];
            potion.IsConsumed = true;
            _inventory.RemoveAt(index);
            return potion;
        }


        // Returns: turns true when the inventory has available space.

        public bool HasFreeSlot()
        {
            // 1) Compare _inventory.Count against MaxSlots.
            // 2) Return whether at least one slot is available.
            return _inventory.Count < MaxSlots;
        }
    }
}
