using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * PotionController.cs — Unity facade for potion brewing (inventory, lifecycle, mutations)
 *
 * What lives here:
 *   - Singleton MonoBehaviour: inventory list, PotionRoller / PotionMixer, selected shelf index, slot cache, events.
 *   - TryRollAndStorePotion, TryMixByIndex, ConsumePotionAt; selection shifts when inventory changes.
 *   - Partial class: PotionInventory.cs (selection + GetAt / cache), PotionUI.cs (Inspector button hooks).
 *
 * Main APIs / usage:
 *   - Instance, Inventory, HasFreeSlot, MaxSlots, Select, mutations above; subscribe to InventoryChanged / SelectionChanged.
 *   - On consume, forwards effect totals to PlayerPotionStats when present.
 */

namespace CrossFade.Potions
{
    public partial class PotionController : MonoBehaviour
    {
        public static PotionController Instance { get; private set; }

        [SerializeField]
        [Tooltip("Max shelf slots (must be > 0).")]
        private int _maxSlots = 8;

        private readonly List<PotionData> _inventory = new();
        private PotionRoller _roller;
        private PotionMixer _mixer;

        // Testing / no selection UI: OnMix pairs newest roll with this potion when still on the shelf (see TryGetTestMixIndices).
        private string _lastMixedInstanceId;

        public int SelectedIndex { get; private set; } = -1;

        private PotionData[] _cachedPotionBySlot;

        public int MaxSlots => _maxSlots;

        public IReadOnlyList<PotionData> Inventory => _inventory;

        public bool HasFreeSlot()
        {
            return _inventory.Count < _maxSlots;
        }

        public event Action InventoryChanged;
        public event Action<int> SelectionChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (_maxSlots <= 0)
            {
                _maxSlots = 8;
            }

            _roller = new PotionRoller();
            _mixer = new PotionMixer();
            _cachedPotionBySlot = new PotionData[_maxSlots];

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            RefreshCache();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnInventoryMutated()
        {
            RefreshCache();
            InventoryChanged?.Invoke();
            InvalidateLastMixedIfMissing();
        }

        private void InvalidateLastMixedIfMissing()
        {
            if (string.IsNullOrEmpty(_lastMixedInstanceId))
            {
                return;
            }

            for (var i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].InstanceId == _lastMixedInstanceId)
                {
                    return;
                }
            }

            _lastMixedInstanceId = null;
        }

        // For testing mix button: newest inventory entry vs last mix result if still present, else the two newest slots.
        private bool TryGetTestMixIndices(out int leftIndex, out int rightIndex)
        {
            leftIndex = rightIndex = -1;
            InvalidateLastMixedIfMissing();
            if (_inventory.Count < 2)
            {
                return false;
            }

            var newestIdx = _inventory.Count - 1;
            var mixedIdx = -1;
            if (!string.IsNullOrEmpty(_lastMixedInstanceId))
            {
                for (var i = 0; i < _inventory.Count; i++)
                {
                    if (_inventory[i].InstanceId == _lastMixedInstanceId)
                    {
                        mixedIdx = i;
                        break;
                    }
                }
            }

            if (mixedIdx >= 0 && mixedIdx != newestIdx)
            {
                leftIndex = Mathf.Min(mixedIdx, newestIdx);
                rightIndex = Mathf.Max(mixedIdx, newestIdx);
                return true;
            }

            leftIndex = newestIdx - 1;
            rightIndex = newestIdx;
            return true;
        }

        private void AfterRemoval(int removedIndex)
        {
            if (SelectedIndex < 0)
            {
                return;
            }

            if (SelectedIndex == removedIndex)
            {
                SelectedIndex = -1;
                SelectionChanged?.Invoke(-1);
            }
            else if (SelectedIndex > removedIndex)
            {
                SelectedIndex--;
                SelectionChanged?.Invoke(SelectedIndex);
            }
        }

        // Same idea as former _manager == null: roller/mixer/cache are created in Awake.
        private bool IsInventoryReady()
        {
            return _roller != null && _mixer != null && _cachedPotionBySlot != null;
        }

        public bool TryRollAndStorePotion()
        {
            if (!IsInventoryReady())
            {
                return false;
            }

            var wasAtCapacity = _inventory.Count >= _maxSlots;

            var potion = _roller.RollPotion();
            if (_inventory.Count >= _maxSlots)
            {
                _inventory.RemoveAt(0);
            }

            _inventory.Add(potion);

            if (wasAtCapacity)
            {
                AfterRemoval(0);
            }

            OnInventoryMutated();
            return true;
        }

        public bool TryMixByIndex(int leftIndex, int rightIndex)
        {
            if (!IsInventoryReady())
            {
                return false;
            }

            try
            {
                ThrowIfInvalidMixIndices(leftIndex, rightIndex);

                var mixedPotion = _mixer.Mix(_inventory[leftIndex], _inventory[rightIndex]);
                var higherIndex = Math.Max(leftIndex, rightIndex);
                var lowerIndex = Math.Min(leftIndex, rightIndex);
                _inventory[lowerIndex] = mixedPotion;
                _inventory.RemoveAt(higherIndex);

                _lastMixedInstanceId = mixedPotion.InstanceId;

                OnInventoryMutated();
                if (SelectedIndex == rightIndex || SelectedIndex == leftIndex)
                {
                    SelectedIndex = Mathf.Min(leftIndex, rightIndex);
                    SelectionChanged?.Invoke(SelectedIndex);
                }

                return true;
            }
            catch (ArgumentException)
            {
                // Invalid indices, duplicate index, or PotionMixer.Mix rules (e.g. consumed potion).
                return false;
            }
        }

        public PotionData ConsumePotionAt(int index)
        {
            if (!IsInventoryReady())
            {
                return null;
            }

            if (index < 0 || index >= _inventory.Count)
            {
                return null;
            }

            var consumed = _inventory[index];
            consumed.IsConsumed = true;
            _inventory.RemoveAt(index);

            if (index < _cachedPotionBySlot.Length)
            {
                _cachedPotionBySlot[index] = null;
            }

            RefreshCache();

            AfterRemoval(index);

            if (PlayerPotionStats.Instance != null)
            {
                PlayerPotionStats.Instance.ApplyConsumedPotion(consumed);
            }

            InventoryChanged?.Invoke();
            return consumed;
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
