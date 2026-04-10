using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrossFade.Potions
{
    // Coordinates potion flow: rolling, mixing, consumption, and slot selection backed by PotionManager.
    // Place on a scene object (e.g. potion room / brewing UI root). Teammates hook UI to InventoryChanged / SelectionChanged;
    // slot clicks call Select(index). Roll / mix buttons call TryRollAndStorePotion / TryMixByIndex.
    // TODO(teammate): Persist inventory across scenes via PlayerStatManager or a save system.
    public class PotionController : MonoBehaviour
    {
        public static PotionController Instance { get; private set; }

        [SerializeField]
        [Tooltip("Max shelf slots passed to PotionManager (must be > 0).")]
        private int _maxSlots = 8;

        private PotionManager _manager;
        private PotionRoller _roller;
        private PotionMixer _mixer;

        // Currently selected inventory index, or -1 if none.
        public int SelectedIndex { get; private set; } = -1;

        // Cached potion per slot when Select() was called so UI still resolves the same instance after list mutations if needed.
        private PotionData[] _cachedPotionBySlot;

        public int MaxSlots => _manager != null ? _manager.MaxSlots : _maxSlots;

        public event Action InventoryChanged;
        public event Action<int> SelectionChanged;
        
        private void Awake()
        {

            Instance = this;

            if (_maxSlots <= 0)
            {
                _maxSlots = 8;
            }

            _roller = new PotionRoller();
            _mixer = new PotionMixer();
            _manager = new PotionManager(_roller, _mixer, _maxSlots);
            _cachedPotionBySlot = new PotionData[_maxSlots];
        }

        
        public void OnRollButtonClicked()
        {


            if (!TryRollAndStorePotion())
            {
                Debug.Log("Roll failed.");
                return;
            }

            var inv = GetInventory();
            var p = inv[inv.Count - 1]; // last added

            var effects = "";
            for (var i = 0; i < p.Effects.Count; i++)
            {
                var e = p.Effects[i];
                effects += $"{e.EffectType}={e.Value} ";
            }

            Debug.Log($"Potion: {p.Name} | Rarity: {p.Rarity}\n{effects}");
        }

        private void Start()
        {
            // TODO(teammate): Replace with Resources.Load, ScriptableObject, or server data
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
        }

        public void Select(int index)
        {
            if (index < -1 || index >= _manager.MaxSlots)
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
            for (var i = 0; i < _cachedPotionBySlot.Length; i++)
            {
                _cachedPotionBySlot[i] = null;
            }

            var inv = _manager.Inventory;
            for (var i = 0; i < inv.Count && i < _cachedPotionBySlot.Length; i++)
            {
                _cachedPotionBySlot[i] = inv[i];
            }
        }

        private PotionData GetAtInternal(int index)
        {
            if (_manager == null)
            {
                return null;
            }

            var inv = _manager.Inventory;
            if (index < 0 || index >= inv.Count)
            {
                return null;
            }

            return inv[index];
        }

        // Potion at inventory index, or null if empty / out of range.
        public PotionData GetAt(int index)
        {
            if (index < 0 || index >= _manager.MaxSlots)
            {
                return null;
            }

            var fromManager = GetAtInternal(index);
            if (fromManager != null)
            {
                return fromManager;
            }

            return index < _cachedPotionBySlot.Length ? _cachedPotionBySlot[index] : null;
        }

        // True if another potion can be rolled into inventory.
        public bool HasFreeSlot()
        {
            return _manager != null && _manager.HasFreeSlot();
        }

        // Read-only view of current potions (same list as PotionManager).
        public IReadOnlyList<PotionData> GetInventory()
        {
            return _manager != null ? _manager.Inventory : Array.Empty<PotionData>();
        }

        // Adds a rolled potion; when at max capacity, discards the oldest (index 0) and appends the new one.
        public bool TryRollAndStorePotion()
        {
            if (_manager == null)
            {
                return false;
            }

            var wasAtCapacity = GetInventory().Count >= MaxSlots;

            var ok = _manager.TryRollAndStorePotion();
            if (ok)
            {
                if (wasAtCapacity)
                {
                    if (SelectedIndex == 0)
                    {
                        SelectedIndex = -1;
                        SelectionChanged?.Invoke(SelectedIndex);
                    }
                    else if (SelectedIndex > 0)
                    {
                        SelectedIndex--;
                        SelectionChanged?.Invoke(SelectedIndex);
                    }
                }

                OnInventoryMutated();
            }

            return ok;
        }

        // Combines two inventory entries; replaces lower index, removes higher.
        public bool TryMixByIndex(int leftIndex, int rightIndex)
        {
            if (_manager == null)
            {
                return false;
            }

            try
            {
                var ok = _manager.TryMixByIndex(leftIndex, rightIndex);
                if (ok)
                {
                    OnInventoryMutated();
                    if (SelectedIndex == rightIndex || SelectedIndex == leftIndex)
                    {
                        // TODO(teammate): Tweak selection rules if mixed indices should focus the result slot.
                        SelectedIndex = Mathf.Min(leftIndex, rightIndex);
                        SelectionChanged?.Invoke(SelectedIndex);
                    }
                }

                return ok;
            }
            catch (ArgumentException)
            {
                // TODO(teammate): Surface validation errors to UI if desired.
                return false;
            }
        }

        // Consumes and removes potion at index; returns it for applying stats / timing. See PotionManager TODO hooks.
        public PotionData ConsumePotionAt(int index)
        {
            if (_manager == null)
            {
                return null;
            }

            PotionData consumed;
            try
            {
                consumed = _manager.ConsumePotion(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            if (index >= 0 && index < _cachedPotionBySlot.Length)
            {
                _cachedPotionBySlot[index] = null;
            }

            // Re-align cache indices after removal (list shifts).
            RefreshCache();

            if (SelectedIndex == index)
            {
                SelectedIndex = -1;
                SelectionChanged?.Invoke(-1);
            }
            else if (SelectedIndex > index)
            {
                SelectedIndex--;
                SelectionChanged?.Invoke(SelectedIndex);
            }

            // TODO(teammate): Apply PotionTiming / PlayerStatsTemp when consuming (call site may move to a dedicated effect applier).
            InventoryChanged?.Invoke();
            return consumed;
        }

        // Clears cached selection for a slot without consuming (e.g. UI cancel). Does not remove from inventory.
        public void ClearSlotCache(int index)
        {
            if (index >= 0 && index < _cachedPotionBySlot.Length)
            {
                _cachedPotionBySlot[index] = null;
            }

            if (SelectedIndex == index)
            {
                SelectedIndex = -1;
                SelectionChanged?.Invoke(-1);
            }
        }
    }
}
