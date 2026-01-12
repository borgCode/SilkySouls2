using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SilkySouls2.Core;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views;

namespace SilkySouls2.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private AttunementWindow _attunementWindow;

        private readonly HotkeyManager _hotkeyManager;
        private readonly UtilityService _utilityService;
        private readonly PlayerViewModel _playerViewModel;
        private readonly IEzStateService _ezStateService;

        private float _desiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;

        private bool _wasNoDeathEnabled;
        private bool _isAttached;

        private readonly Dictionary<int, AttunementSpell> _spellLookup;

        public UtilityViewModel(UtilityService utilityService, HotkeyManager hotkeyManager,
            PlayerViewModel playerViewModel, GameStateService gameStateService, IEzStateService ezStateService)
        {
            _playerViewModel = playerViewModel;
            _ezStateService = ezStateService;
            _utilityService = utilityService;
            _hotkeyManager = hotkeyManager;

            _spellLookup = DataLoader.GetAttunementSpells();

            AvailableSpells = new ObservableCollection<InventorySpell>();
            EquippedSpells = new ObservableCollection<EquippedSpell>();

            gameStateService.Subscribe(GameState.FirstLoaded, OnGameFirstLoaded);
            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);
            gameStateService.Subscribe(GameState.Detached, OnGameDetached);
            gameStateService.Subscribe(GameState.Attached, OnGameAttached);
            gameStateService.Subscribe(GameState.AppStart, OnAppStart);

            RegisterHotkeys();

            ForceSaveCommand = new DelegateCommand(ForceSave);
            OpenAttunementCommand = new DelegateCommand(OpenAttunementWindow);
        }
        

        #region Commands

        public ICommand ForceSaveCommand { get; set; }
        public ICommand OpenAttunementCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isDrawHitboxEnabled;

        public bool IsDrawHitboxEnabled
        {
            get => _isDrawHitboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
                _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
            }
        }

        private bool _isDrawEventEnabled;

        public bool IsDrawEventEnabled
        {
            get => _isDrawEventEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventEnabled, value)) return;
                IsDrawEventGeneralEnabled = _isDrawEventEnabled;
                IsDrawEventSpawnEnabled = _isDrawEventEnabled;
                IsDrawEventInvasionEnabled = _isDrawEventEnabled;
                IsDrawEventLeashEnabled = _isDrawEventEnabled;
                if (!_isDrawEventEnabled) IsDrawEventOtherEnabled = false;
            }
        }

        private bool _isDrawEventGeneralEnabled;

        public bool IsDrawEventGeneralEnabled
        {
            get => _isDrawEventGeneralEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventGeneralEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventGeneral, _isDrawEventGeneralEnabled);
            }
        }

        private bool _isDrawEventSpawnEnabled;

        public bool IsDrawEventSpawnEnabled
        {
            get => _isDrawEventSpawnEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventSpawnEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventSpawn, _isDrawEventSpawnEnabled);
            }
        }

        private bool _isDrawEventInvasionEnabled;

        public bool IsDrawEventInvasionEnabled
        {
            get => _isDrawEventInvasionEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventInvasionEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventInvasion, _isDrawEventInvasionEnabled);
            }
        }

        private bool _isDrawEventLeashEnabled;

        public bool IsDrawEventLeashEnabled
        {
            get => _isDrawEventLeashEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventLeashEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventLeash, _isDrawEventLeashEnabled);
            }
        }

        private bool _isDrawEventOtherEnabled;

        public bool IsDrawEventOtherEnabled
        {
            get => _isDrawEventOtherEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventOtherEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventOther, _isDrawEventOtherEnabled);
            }
        }

        private bool _isDrawSoundEnabled;

        public bool IsDrawSoundEnabled
        {
            get => _isDrawSoundEnabled;
            set
            {
                if (!SetProperty(ref _isDrawSoundEnabled, value)) return;
                _utilityService.ToggleDrawSound(_isDrawSoundEnabled);
            }
        }

        private bool _isTargetingViewEnabled;

        public bool IsTargetingViewEnabled
        {
            get => _isTargetingViewEnabled;
            set
            {
                if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
                _utilityService.ToggleTargetingView(_isTargetingViewEnabled);
            }
        }

        private bool _isDrawRagdollEnabled;

        public bool IsDrawRagdollsEnabled
        {
            get => _isDrawRagdollEnabled;
            set
            {
                if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
                _utilityService.ToggleRagdoll(_isDrawRagdollEnabled);
                if (!_isDrawRagdollEnabled) IsSeeThroughWallsEnabled = false;
            }
        }

        private bool _isSeeThroughwallsEnabled;

        public bool IsSeeThroughWallsEnabled
        {
            get => _isSeeThroughwallsEnabled;
            set
            {
                if (!SetProperty(ref _isSeeThroughwallsEnabled, value)) return;
                _utilityService.ToggleRagdollEsp(_isSeeThroughwallsEnabled);
            }
        }

        private bool _isDrawCollisionEnabled;

        public bool IsDrawCollisionEnabled
        {
            get => _isDrawCollisionEnabled;
            set
            {
                if (!SetProperty(ref _isDrawCollisionEnabled, value)) return;
                _utilityService.ToggleDrawCol(_isDrawCollisionEnabled);
                if (!_isDrawCollisionEnabled) IsColWireframeEnabled = false;
            }
        }

        private bool _isColWireframeEnabled;

        public bool IsColWireframeEnabled
        {
            get => _isColWireframeEnabled;
            set
            {
                if (!SetProperty(ref _isColWireframeEnabled, value)) return;
                _utilityService.ToggleColWireframe(_isColWireframeEnabled);
            }
        }

        private bool _isDrawKillboxEnabled;

        public bool IsDrawKillboxEnabled
        {
            get => _isDrawKillboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawKillboxEnabled, value)) return;
                _utilityService.ToggleDrawKillbox(_isDrawKillboxEnabled);
            }
        }

        private bool _isDrawObjEnabled;

        public bool IsDrawObjEnabled
        {
            get => _isDrawObjEnabled;
            set
            {
                if (!SetProperty(ref _isDrawObjEnabled, value)) return;
                _utilityService.ToggleDrawObj(_isDrawObjEnabled);
            }
        }

        private bool _isHideCharactersEnabled;

        public bool IsHideCharactersEnabled
        {
            get => _isHideCharactersEnabled;
            set
            {
                if (!SetProperty(ref _isHideCharactersEnabled, value)) return;
                _utilityService.ToggleHideChr(_isHideCharactersEnabled);
            }
        }

        private bool _isHideMapEnabled;

        public bool IsHideMapEnabled
        {
            get => _isHideMapEnabled;
            set
            {
                if (!SetProperty(ref _isHideMapEnabled, value)) return;
                _utilityService.ToggleHideMap(_isHideMapEnabled);
            }
        }

        private bool _isLightGutterEnabled;

        public bool IsLightGutterEnabled
        {
            get => _isLightGutterEnabled;
            set
            {
                if (!SetProperty(ref _isLightGutterEnabled, value)) return;
                _utilityService.ToggleLightGutter(_isLightGutterEnabled);
            }
        }

        private bool _isNoFogEnabled;

        public bool IsNoFogEnabled
        {
            get => _isNoFogEnabled;
            set
            {
                if (!SetProperty(ref _isNoFogEnabled, value)) return;
                _utilityService.ToggleShadedFog(_isNoFogEnabled);
            }
        }

        private bool _isCreditSkipEnabled;

        public bool IsCreditSkipEnabled
        {
            get => _isCreditSkipEnabled;
            set
            {
                if (!SetProperty(ref _isCreditSkipEnabled, value)) return;
                _utilityService.ToggleCreditSkip(_isCreditSkipEnabled);
            }
        }

        private bool _is100DropEnabled;

        public bool Is100DropEnabled
        {
            get => _is100DropEnabled;
            set
            {
                if (!SetProperty(ref _is100DropEnabled, value)) return;
                _utilityService.Toggle100Drop(_is100DropEnabled);
            }
        }

        private bool _isNoClipEnabled;

        public bool IsNoClipEnabled
        {
            get => _isNoClipEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipEnabled, value)) return;

                if (_isNoClipEnabled)
                {
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                    _wasNoDeathEnabled = _playerViewModel.IsNoDeathEnabled;
                    _playerViewModel.IsNoDeathEnabled = true;
                    _utilityService.ToggleKillboxHook(true);
                }
                else
                {
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                    _playerViewModel.IsNoDeathEnabled = _wasNoDeathEnabled;
                    NoClipSpeed = DefaultNoclipMultiplier;
                    _utilityService.ToggleKillboxHook(false);
                }
            }
        }

        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;

        public float NoClipSpeed
        {
            get => _noClipSpeedMultiplier;
            set
            {
                if (SetProperty(ref _noClipSpeedMultiplier, value))
                {
                    SetNoClipSpeed(value);
                }
            }
        }

        private float _gameSpeed = 1.0f;

        public float GameSpeed
        {
            get => _gameSpeed;
            set
            {
                if (SetProperty(ref _gameSpeed, value))
                {
                    _utilityService.SetGameSpeed(value);
                    if (IsRememberGameSpeedEnabled && Math.Abs(value - DefaultSpeed) > Epsilon)
                    {
                        SettingsManager.Default.GameSpeed = value;
                    }
                }
            }
        }
        
        private bool _isRememberGameSpeedEnabled;

        public bool IsRememberGameSpeedEnabled
        {
            get => _isRememberGameSpeedEnabled;
            set
            {
                if (!SetProperty(ref _isRememberGameSpeedEnabled, value)) return;
                if (_isRememberGameSpeedEnabled)
                {
                    SettingsManager.Default.RememberGameSpeed = _isRememberGameSpeedEnabled;

                    if (Math.Abs(GameSpeed - DefaultSpeed) > Epsilon)
                    {
                        SettingsManager.Default.GameSpeed = GameSpeed;
                    }
                }
                else
                {
                    SettingsManager.Default.GameSpeed = DefaultSpeed;
                    SettingsManager.Default.RememberGameSpeed = _isRememberGameSpeedEnabled;
                }
            }
        }

        private ObservableCollection<InventorySpell> _availableSpells;

        public ObservableCollection<InventorySpell> AvailableSpells
        {
            get => _availableSpells;
            private set => SetProperty(ref _availableSpells, value);
        }

        private ObservableCollection<EquippedSpell> _equippedSpells;

        public ObservableCollection<EquippedSpell> EquippedSpells
        {
            get => _equippedSpells;
            private set => SetProperty(ref _equippedSpells, value);
        }

        private int _numOfSlots;

        public int NumOfSlots
        {
            get => _numOfSlots;
            private set => SetProperty(ref _numOfSlots, value);
        }

        public int EquippedSlotsRows => (int)Math.Ceiling(NumOfSlots / 7.0);
        public bool HasAttunementSlots => NumOfSlots > 0;
        public bool HasSpellsInInventory => AvailableSpells?.Count > 0;

        #endregion

        #region Private Methods

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.ForceSave, () =>
            {
                if (!AreOptionsEnabled) return;
                _utilityService.ForceSave();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.NoClip, () => { IsNoClipEnabled = !IsNoClipEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseNoClipSpeed, () =>
            {
                if (IsNoClipEnabled)
                    NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
            });

            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseNoClipSpeed, () =>
            {
                if (IsNoClipEnabled)
                    NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
            });
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleGameSpeed, ToggleSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed,
                () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseGameSpeed,
                () => SetSpeed(Math.Max(0, GameSpeed - 0.50f)));
        }

        private void SetNoClipSpeed(float multiplier)
        {
            if (!IsNoClipEnabled) return;
            if (multiplier < 0.05f) multiplier = 0.05f;
            else if (multiplier > 5.0f) multiplier = 5.0f;

            SetProperty(ref _noClipSpeedMultiplier, multiplier);

            float baseXFloat = BitConverter.ToSingle(BitConverter.GetBytes(BaseXSpeedHex), 0);
            float baseYFloat = BitConverter.ToSingle(BitConverter.GetBytes(BaseYSpeedHex), 0);

            float newXFloat = baseXFloat * multiplier;
            float newYFloat = baseYFloat * multiplier;

            byte[] xBytes = BitConverter.GetBytes(newXFloat);
            byte[] yBytes = BitConverter.GetBytes(newYFloat);

            _utilityService.SetNoClipSpeed(xBytes, yBytes);
        }

        private void ToggleSpeed()
        {
            if (!_isAttached) return;

            if (!IsApproximately(GameSpeed, DefaultSpeed))
            {
                _desiredSpeed = GameSpeed;
                SetSpeed(DefaultSpeed);
            }
            else if (_desiredSpeed >= 0)
            {
                SetSpeed(_desiredSpeed);
            }
        }

        private bool IsApproximately(float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        private void OnGameLoaded()
        {
            if (IsCreditSkipEnabled) _utilityService.ToggleCreditSkip(true);
            if (_attunementWindow != null && _attunementWindow.IsVisible) RefreshSpells();
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            IsNoClipEnabled = false;
            AreOptionsEnabled = false;
        }

        private void OnGameFirstLoaded()
        {
            if (Is100DropEnabled) _utilityService.Toggle100Drop(true);
            if (IsCreditSkipEnabled) _utilityService.ToggleCreditSkip(true);
            if (IsDrawHitboxEnabled) _utilityService.ToggleDrawHitbox(true);

            if (IsDrawEventGeneralEnabled) _utilityService.ToggleDrawEvent(DrawType.EventGeneral, true);
            if (IsDrawEventSpawnEnabled) _utilityService.ToggleDrawEvent(DrawType.EventSpawn, true);
            if (IsDrawEventInvasionEnabled) _utilityService.ToggleDrawEvent(DrawType.EventInvasion, true);
            if (IsDrawEventLeashEnabled) _utilityService.ToggleDrawEvent(DrawType.EventLeash, true);
            if (IsDrawEventOtherEnabled) _utilityService.ToggleDrawEvent(DrawType.EventOther, true);

            if (IsDrawSoundEnabled) _utilityService.ToggleDrawSound(true);
            if (IsTargetingViewEnabled) _utilityService.ToggleTargetingView(true);
            if (IsHideMapEnabled) _utilityService.ToggleHideMap(true);
            if (IsHideCharactersEnabled) _utilityService.ToggleHideChr(true);
            if (IsLightGutterEnabled) _utilityService.ToggleLightGutter(true);
            if (IsDrawCollisionEnabled) _utilityService.ToggleDrawCol(true);
            if (IsNoFogEnabled) _utilityService.ToggleShadedFog(true);
            if (IsColWireframeEnabled) _utilityService.ToggleColWireframe(true);
            if (IsDrawKillboxEnabled) _utilityService.ToggleDrawKillbox(true);
            if (IsDrawRagdollsEnabled) _utilityService.ToggleRagdoll(true);
            if (IsSeeThroughWallsEnabled) _utilityService.ToggleRagdollEsp(true);
        }

        private void OnGameDetached()
        {
            _gameSpeed = 1.0f;
            OnPropertyChanged(nameof(GameSpeed));
            _utilityService.Reset();
            _isAttached = false;
        }

        private void OnGameAttached()
        {
            _isAttached = true;
        }
        
        private void OnAppStart()
        {
            _isRememberGameSpeedEnabled = SettingsManager.Default.RememberGameSpeed;
            OnPropertyChanged(nameof(IsRememberGameSpeedEnabled));
            if (_isRememberGameSpeedEnabled) _desiredSpeed = SettingsManager.Default.GameSpeed;
        }

        private void OpenAttunementWindow()
        {
            if (_attunementWindow != null && _attunementWindow.IsVisible)
            {
                _attunementWindow.Activate();
                return;
            }

            NumOfSlots = _utilityService.GetTotalAvailableSlots();
            RefreshSpells();
            _attunementWindow = new AttunementWindow
            {
                DataContext = this
            };

            _attunementWindow.Closed += (s, e) => _attunementWindow = null;

            _attunementWindow.Show();
        }

        private int GetFirstAvailableSlot()
        {
            for (int i = 0; i < EquippedSpells.Count; i++)
            {
                if (EquippedSpells[i].Id <= 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void RefreshSpells()
        {
            NumOfSlots = _utilityService.GetTotalAvailableSlots();
            var inventorySpells = _utilityService.GetInventorySpells();
            var equippedSpells = _utilityService.GetEquippedSpells();

            var actualEquipped = equippedSpells.Take(NumOfSlots).ToList();

            foreach (var inventorySpell in inventorySpells)
            {
                if (_spellLookup.TryGetValue(inventorySpell.Id, out AttunementSpell spell))
                {
                    inventorySpell.Name = spell.Name;
                    inventorySpell.Type = spell.Type;
                }
                else
                {
                    inventorySpell.Name = "Unknown";
                    inventorySpell.Type = SpellType.Hex;
                }
            }

            foreach (var equippedSpell in actualEquipped)
            {
                if (equippedSpell.Id <= 0) equippedSpell.Name = "Empty Slot";
                else
                    equippedSpell.Name = _spellLookup.TryGetValue(equippedSpell.Id, out AttunementSpell spell)
                        ? spell.Name
                        : "Unknown";
            }

            EquippedSpells.Clear();
            foreach (var spell in actualEquipped)
            {
                EquippedSpells.Add(spell);
            }

            var sortedInventorySpells = inventorySpells
                .OrderBy(s => _spellLookup.TryGetValue(s.Id, out AttunementSpell spell) ? spell.Type : SpellType.Hex)
                .ThenBy(s => s.Name)
                .ToList();
            AvailableSpells.Clear();
            foreach (var spell in sortedInventorySpells)
            {
                AvailableSpells.Add(spell);
            }

            OnPropertyChanged(nameof(EquippedSlotsRows));
            OnPropertyChanged(nameof(HasAttunementSlots));
            OnPropertyChanged(nameof(HasSpellsInInventory));
        }

        private void ForceSave() => _utilityService.ForceSave();
        private void SetSpeed(float value) => GameSpeed = value;

        #endregion

        #region Public Methods

        public async void HandleSpellAttune(InventorySpell spell)
        {
            if (!AreOptionsEnabled) return;
            int emptySlots = EquippedSpells.Count(s => s.Id <= 0);

            if (spell.SlotReq > emptySlots)
            {
                MessageBox.Show($"Cannot attune spell. Requires {spell.SlotReq} slots but only {emptySlots} available.",
                    "Insufficient Slots", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int slotIndex = GetFirstAvailableSlot();
            if (slotIndex != -1)
            {
                _utilityService.AttuneSpell(slotIndex, spell.EntryAddress);
                await Task.Delay(50);
                RefreshSpells();
            }
        }

        public async void HandleUnAttune(int slotIndex)
        {
            if (!AreOptionsEnabled) return;
            _utilityService.AttuneSpell(slotIndex, IntPtr.Zero);
            await Task.Delay(50);
            RefreshSpells();
        }

        #endregion
    }
}