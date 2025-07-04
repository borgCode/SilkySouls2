﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views;

namespace SilkySouls2.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private bool _isCreditSkipEnabled;
        private bool _is100DropEnabled;
        private bool _isMadWarriorSpawnEnabled;
        private string _madWarriorStatusText;
        private Brush _madWarriorStatusColor;
        
        private bool _isDrawHitboxEnabled;
        private bool _isDrawEventEnabled;
        private bool _isDrawEventGeneralEnabled;
        private bool _isDrawEventSpawnEnabled;
        private bool _isDrawEventInvasionEnabled;
        private bool _isDrawEventLeashEnabled;
        private bool _isDrawEventOtherEnabled;


        private bool _isDrawSoundEnabled;
        private bool _isTargetingViewEnabled;
        private bool _isDrawRagdollEnabled;
        private bool _isSeeThroughwallsEnabled;
        private bool _isDrawCollisionEnabled;
        private bool _isColWireframeEnabled;
        private bool _isDrawKillboxEnabled;


        private bool _isHideCharactersEnabled;
        private bool _isHideMapEnabled;
        private bool _isLightGutterEnabled;
        private bool _isNoFogEnabled;

        private AttunementWindow _attunementWindow;

        private bool _areButtonsEnabled;
        private readonly HotkeyManager _hotkeyManager;
        private readonly UtilityService _utilityService;
        private readonly PlayerViewModel _playerViewModel;

        private float _gameSpeed = 1.0f;
        
        private float _desiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;
        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;
        private bool _isNoClipEnabled;
        private bool _wasNoDeathEnabled;
        private bool _isAttached;


        private Dictionary<int, AttunementSpell> _spellLookup = new Dictionary<int, AttunementSpell>();


        public UtilityViewModel(UtilityService utilityService, HotkeyManager hotkeyManager,
            PlayerViewModel playerViewModel)
        {
            _playerViewModel = playerViewModel;
            _utilityService = utilityService;
            _hotkeyManager = hotkeyManager;

            _spellLookup = DataLoader.GetAttunementSpells();
            
            AvailableSpells = new ObservableCollection<InventorySpell>();
            EquippedSpells = new ObservableCollection<EquippedSpell>();

            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("ForceSave", () =>
            {
                if (!AreButtonsEnabled) return;
                _utilityService.ForceSave();
            });
            _hotkeyManager.RegisterAction("NoClip", () => { IsNoClipEnabled = !IsNoClipEnabled; });
            _hotkeyManager.RegisterAction("IncreaseNoClipSpeed", () =>
            {
                if (IsNoClipEnabled)
                    NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
            });

            _hotkeyManager.RegisterAction("DecreaseNoClipSpeed", () =>
            {
                if (IsNoClipEnabled)
                    NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
            });
            _hotkeyManager.RegisterAction("ToggleGameSpeed", ToggleSpeed);
            _hotkeyManager.RegisterAction("IncreaseGameSpeed", () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction("DecreaseGameSpeed", () => SetSpeed(Math.Max(0, GameSpeed - 0.50f)));
        }


        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set => SetProperty(ref _areButtonsEnabled, value);
        }
        
        
        public bool IsMadWarriorSpawnEnabled
        {
            get => _isMadWarriorSpawnEnabled;
            set
            {
                if (!SetProperty(ref _isMadWarriorSpawnEnabled, value)) return;
                if (_isMadWarriorSpawnEnabled)
                {
                    UpdateMadWarriorStatus();
                }
                else
                {
                    MadWarriorStatusText = "";
                }
                
            }
        }

        public string MadWarriorStatusText
        {
            get => _madWarriorStatusText;
            set => SetProperty(ref _madWarriorStatusText, value);
        }

        public Brush MadWarriorStatusColor
        {
            get => _madWarriorStatusColor;
            set => SetProperty(ref _madWarriorStatusColor, value);
        }

        public bool IsDrawHitboxEnabled
        {
            get => _isDrawHitboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
                _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
            }
        }

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

        public bool IsDrawEventGeneralEnabled
        {
            get => _isDrawEventGeneralEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventGeneralEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventGeneral, _isDrawEventGeneralEnabled);
            }
        }

        public bool IsDrawEventSpawnEnabled
        {
            get => _isDrawEventSpawnEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventSpawnEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventSpawn, _isDrawEventSpawnEnabled);
            }
        }

        public bool IsDrawEventInvasionEnabled
        {
            get => _isDrawEventInvasionEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventInvasionEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventInvasion, _isDrawEventInvasionEnabled);
            }
        }

        public bool IsDrawEventLeashEnabled
        {
            get => _isDrawEventLeashEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventLeashEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventLeash, _isDrawEventLeashEnabled);
            }
        }
        
        public bool IsDrawEventOtherEnabled
        {
            get => _isDrawEventOtherEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventOtherEnabled, value)) return;
                _utilityService.ToggleDrawEvent(DrawType.EventOther, _isDrawEventOtherEnabled);
            }
        }

        public bool IsDrawSoundEnabled
        {
            get => _isDrawSoundEnabled;
            set
            {
                if (!SetProperty(ref _isDrawSoundEnabled, value)) return;
                _utilityService.ToggleDrawSound(_isDrawSoundEnabled);
            }
        }

        public bool IsTargetingViewEnabled
        {
            get => _isTargetingViewEnabled;
            set
            {
                if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
                _utilityService.ToggleTargetingView(_isTargetingViewEnabled);
            }
        }


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

        public bool IsSeeThroughWallsEnabled
        {
            get => _isSeeThroughwallsEnabled;
            set
            {
                if (!SetProperty(ref _isSeeThroughwallsEnabled, value)) return;
                _utilityService.ToggleRagdollEsp(_isSeeThroughwallsEnabled);
            }
        }

        public bool IsColWireframeEnabled
        {
            get => _isColWireframeEnabled;
            set
            {
                if (!SetProperty(ref _isColWireframeEnabled, value)) return;
                _utilityService.ToggleColWireframe(_isColWireframeEnabled);
            }
        }

        public bool IsDrawKillboxEnabled
        {
            get => _isDrawKillboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawKillboxEnabled, value)) return;
                _utilityService.ToggleDrawKillbox(_isDrawKillboxEnabled);
            }
        }


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


        public bool IsHideCharactersEnabled
        {
            get => _isHideCharactersEnabled;
            set
            {
                if (!SetProperty(ref _isHideCharactersEnabled, value)) return;
                _utilityService.ToggleHideChr(_isHideCharactersEnabled);
            }
        }

        public bool IsHideMapEnabled
        {
            get => _isHideMapEnabled;
            set
            {
                if (!SetProperty(ref _isHideMapEnabled, value)) return;
                _utilityService.ToggleHideMap(_isHideMapEnabled);
            }
        }
        
        public bool IsLightGutterEnabled
        {
            get => _isLightGutterEnabled;
            set
            {
                if (!SetProperty(ref _isLightGutterEnabled, value)) return;
                _utilityService.ToggleLightGutter(_isLightGutterEnabled);
            }
        }
        
        public bool IsNoFogEnabled
        {
            get => _isNoFogEnabled;
            set
            {
                if (!SetProperty(ref _isNoFogEnabled, value)) return;
                _utilityService.ToggleShadedFog(_isNoFogEnabled);
            }
        }

        public bool IsCreditSkipEnabled
        {
            get => _isCreditSkipEnabled;
            set
            {
                if (!SetProperty(ref _isCreditSkipEnabled, value)) return;
                _utilityService.ToggleCreditSkip(_isCreditSkipEnabled);
            }
        }


        public bool Is100DropEnabled
        {
            get => _is100DropEnabled;
            set
            {
                if (!SetProperty(ref _is100DropEnabled, value)) return;
                _utilityService.Toggle100Drop(_is100DropEnabled);
            }
        }

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

        public void SetNoClipSpeed(float multiplier)
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

        public float GameSpeed
        {
            get => _gameSpeed;
            set
            {
                if (SetProperty(ref _gameSpeed, value))
                {
                    _utilityService.SetGameSpeed(value);
                }
            }
        }

        public void SetSpeed(float value) => GameSpeed = value;


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

        public void TryEnableFeatures()
        {
            if (IsCreditSkipEnabled) _utilityService.ToggleCreditSkip(true);
            if (_attunementWindow != null && _attunementWindow.IsVisible) RefreshSpells();
            AreButtonsEnabled = true;
        }

        public void DisableFeatures()
        {
            IsNoClipEnabled = false;
            AreButtonsEnabled = false;
        }

        public void ForceSave() => _utilityService.ForceSave();


        public void TryApplyOneTimeFeatures()
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
        
        public void RefreshSpells()
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
                else equippedSpell.Name = _spellLookup.TryGetValue(equippedSpell.Id, out AttunementSpell spell) ? spell.Name : "Unknown";
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

        public void Test()
        {
            _utilityService.Test();
        }
        
        
        public void OpenAttunementWindow()
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
        
        public async void HandleSpellAttune(InventorySpell spell)
        {
            if (!AreButtonsEnabled) return;
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
            if (!AreButtonsEnabled) return;
            _utilityService.AttuneSpell(slotIndex, IntPtr.Zero);
            await Task.Delay(50);
            RefreshSpells();
        }

        public void Reset()
        {
            _gameSpeed = 1.0f;
            OnPropertyChanged(nameof(GameSpeed));
            _utilityService.Reset();
            _isAttached = false;
        }

        public void ApplyLaunchFeatures()
        {
            _isAttached = true;
        }

        public void ApplyDelayedFeatures()
        {
            if (IsMadWarriorSpawnEnabled) UpdateMadWarriorStatus();
        }

        private void UpdateMadWarriorStatus()
        {
            if (!AreButtonsEnabled) return;
            if (_utilityService.GetEvent(GameIds.EventFlags.MadWarriorSpawn))
            {
                MadWarriorStatusText = "Spawned";
                MadWarriorStatusColor = Brushes.Chartreuse;
            }
            else
            {
                MadWarriorStatusText = "Not Spawned";
                MadWarriorStatusColor = Brushes.Red;
            }
        }
    }
}