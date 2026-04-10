using System;
using System.Collections.Generic;

namespace CrossFade.Potions
{
    public class PotionManager
    {
        private readonly PotionRoller _roller;
        private readonly PotionMixer _mixer;
        private readonly List<PotionData> _inventory = new();

        public int MaxSlots { get; private set; }

        public IReadOnlyList<PotionData> Inventory => _inventory;

        public PotionManager(PotionRoller roller, PotionMixer mixer, int maxSlots)
        {
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

        public bool TryRollAndStorePotion()
        {
            var potion = _roller.RollPotion();
            if (_inventory.Count >= MaxSlots)
            {
                _inventory.RemoveAt(0);
            }

            _inventory.Add(potion);
            return true;
        }

        public bool TryMixByIndex(int leftIndex, int rightIndex)
        {
            ThrowIfInvalidMixIndices(leftIndex, rightIndex);

            var leftPotion = _inventory[leftIndex];
            var rightPotion = _inventory[rightIndex];
            var mixedPotion = _mixer.Mix(leftPotion, rightPotion);
            var higherIndex = Math.Max(leftIndex, rightIndex);
            var lowerIndex = Math.Min(leftIndex, rightIndex);
            _inventory[lowerIndex] = mixedPotion;
            _inventory.RemoveAt(higherIndex);
            return true;
        }

        public PotionData ConsumePotion(int index)
        {
            if (index < 0 || index >= _inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var potion = _inventory[index];
            potion.IsConsumed = true;
            _inventory.RemoveAt(index);

            // TODO(teammate): Apply potion effect totals to shared PlayerStats and timer systems here.

            return potion;
        }

        public bool HasFreeSlot()
        {
            return _inventory.Count < MaxSlots;
        }

        private void ThrowIfInvalidMixIndices(int leftIndex, int rightIndex)
        {
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
        }
    }
}
