using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using H.Hooks;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _currentSettingHotkeyId;
        private LowLevelKeyboardHook _tempHook;
        private Keys _currentKeys;

        private readonly Dictionary<string, HotkeyBindingViewModel> _hotkeyLookup;

        private readonly ISettingsService _settingsService;
        private readonly HotkeyManager _hotkeyManager;

        private bool _isLoaded;

        public ObservableCollection<HotkeyBindingViewModel> PlayerHotkeys { get; }
        public ObservableCollection<HotkeyBindingViewModel> EnemiesHotkeys { get; }
        public ObservableCollection<HotkeyBindingViewModel> TargetHotkeys { get; }
        public ObservableCollection<HotkeyBindingViewModel> UtilityHotkeys { get; }

        public SettingsViewModel(ISettingsService settingsService, HotkeyManager hotkeyManager,
            GameStateService gameStateService)
        {
            _settingsService = settingsService;
            _hotkeyManager = hotkeyManager;

            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.AppStart, OnAppStart);

            RegisterHotkeys();


            PlayerHotkeys =
            [
                new("Save Position 1", HotkeyActions.SavePos1),
                new("Save Position 2", HotkeyActions.SavePos2),
                new("Restore Position 1", HotkeyActions.RestorePos1),
                new("Restore Position 2", HotkeyActions.RestorePos2),
                new("RTSR", HotkeyActions.RTSR),
                new("Max Hp", HotkeyActions.MaxHp),
                new("Set Custom Hp", HotkeyActions.PlayerSetCustomHp),
                new("No Death", HotkeyActions.NoDeath),
                new("One Shot", HotkeyActions.OneShot),
                new("Deal no Damage", HotkeyActions.DealNoDamage),
                new("No Damage", HotkeyActions.PlayerNoDamage),
                new("Restore Spellcasts", HotkeyActions.RestoreSpellcasts),
                new("Restore Humanity", HotkeyActions.RestoreHumanity),
                new("Rest Character", HotkeyActions.RestCharacter),
                new("Toggle Player Speed", HotkeyActions.TogglePlayerSpeed),
                new("Increase Player Speed", HotkeyActions.IncreasePlayerSpeed),
                new("Decrease Player Speed", HotkeyActions.DecreasePlayerSpeed),
            ];
            TargetHotkeys =
            [
                new("Enable Target Options", HotkeyActions.EnableTargetOptions),
                new("Show All Resistances", HotkeyActions.ShowAllResistances),
                new("Freeze Target HP", HotkeyActions.FreezeHp),
                new("Kill Target", HotkeyActions.KillTarget),
                new("Disable Target AI", HotkeyActions.DisableTargetAi),
                new("Increase Target Speed", HotkeyActions.IncreaseTargetSpeed),
                new("Decrease Target Speed", HotkeyActions.DecreaseTargetSpeed),
                new("Repeat Act", HotkeyActions.TargetRepeatAct),
                
            ];
            
            EnemiesHotkeys =
            [
                new("Disable All AI", HotkeyActions.DisableAi),
            ];

            UtilityHotkeys =
            [
                new("Quitout", HotkeyActions.Quitout),
                new("Force Save", HotkeyActions.ForceSave),
                new("Warp", HotkeyActions.Warp),
                new("Toggle Game Speed", HotkeyActions.ToggleGameSpeed),
                new("Increase Game Speed", HotkeyActions.IncreaseGameSpeed),
                new("Decrease Game Speed", HotkeyActions.DecreaseGameSpeed),
                new("No Clip", HotkeyActions.NoClip),
                new("Increase No Clip Speed", HotkeyActions.IncreaseNoClipSpeed),
                new("Decrease No Clip Speed", HotkeyActions.DecreaseNoClipSpeed),
            ];
            
            _hotkeyLookup = PlayerHotkeys
                .Concat(EnemiesHotkeys)
                .Concat(TargetHotkeys)
                .Concat(UtilityHotkeys)
                .ToDictionary(h => h.ActionId);


            LoadHotkeyDisplays();
        }

        #region Properties

        private bool _isEnableHotkeysEnabled;

        public bool IsEnableHotkeysEnabled
        {
            get => _isEnableHotkeysEnabled;
            set
            {
                if (SetProperty(ref _isEnableHotkeysEnabled, value))
                {
                    SettingsManager.Default.EnableHotkeys = value;
                    SettingsManager.Default.Save();
                    if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
                    else _hotkeyManager.Stop();
                }
            }
        }

        private bool _isFastQuitoutEnabled;

        public bool IsFastQuitoutEnabled
        {
            get => _isFastQuitoutEnabled;
            set
            {
                if (!SetProperty(ref _isFastQuitoutEnabled, value)) return;
                SettingsManager.Default.FastQuitout = value;
                SettingsManager.Default.Save();
                if (_isLoaded)
                {
                    _settingsService.ToggleFastQuitout(_isFastQuitoutEnabled);
                }
            }
        }

        private bool _isBabyJumpFixedEnabled;

        public bool IsBabyJumpFixEnabled
        {
            get => _isBabyJumpFixedEnabled;
            set
            {
                if (!SetProperty(ref _isBabyJumpFixedEnabled, value)) return;
                SettingsManager.Default.BabyJump = value;
                SettingsManager.Default.Save();
                if (_isLoaded)
                {
                    _settingsService.ToggleBabyJumpFix(_isBabyJumpFixedEnabled);
                }
            }
        }

        private bool _isDisableDoubleClickEnabled;

        public bool IsDisableDoubleClickEnabled
        {
            get => _isDisableDoubleClickEnabled;
            set
            {
                if (!SetProperty(ref _isDisableDoubleClickEnabled, value)) return;
                SettingsManager.Default.DoubleClick = value;
                SettingsManager.Default.Save();
                if (_isLoaded)
                {
                    _settingsService.ToggleDoubleClick(_isDisableDoubleClickEnabled);
                }
            }
        }

        private bool _isAlwaysOnTopEnabled;

        public bool IsAlwaysOnTopEnabled
        {
            get => _isAlwaysOnTopEnabled;
            set
            {
                if (!SetProperty(ref _isAlwaysOnTopEnabled, value)) return;
                SettingsManager.Default.AlwaysOnTop = value;
                SettingsManager.Default.Save();
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null) mainWindow.Topmost = _isAlwaysOnTopEnabled;
            }
        }

        #endregion

        #region Public Methods

        public void StartSettingHotkey(string actionId)
        {
            if (_currentSettingHotkeyId != null &&
                _hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var prev))
            {
                prev.HotkeyText = GetHotkeyDisplayText(_currentSettingHotkeyId);
            }

            _currentSettingHotkeyId = actionId;

            if (_hotkeyLookup.TryGetValue(actionId, out var current))
            {
                current.HotkeyText = "Press keys...";
            }

            _tempHook = new LowLevelKeyboardHook();
            _tempHook.IsExtendedMode = true;
            _tempHook.Down += TempHook_Down;
            _tempHook.Start();
        }

        public void ConfirmHotkey()
        {
            var currentSettingHotkeyId = _currentSettingHotkeyId;
            var currentKeys = _currentKeys;
            if (currentSettingHotkeyId == null || currentKeys == null || currentKeys.IsEmpty)
            {
                CancelSettingHotkey();
                return;
            }

            HandleExistingHotkey(currentKeys);
            SetNewHotkey(currentSettingHotkeyId, currentKeys);

            StopSettingHotkey();
        }

        public void CancelSettingHotkey()
        {
            var actionId = _currentSettingHotkeyId;

            if (actionId != null && _hotkeyLookup.TryGetValue(actionId, out var binding))
            {
                binding.HotkeyText = "None";
                _hotkeyManager.SetHotkey(actionId, new Keys());
            }

            StopSettingHotkey();
        }

        #endregion

        #region Private Methods

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.Quitout, () => _settingsService.Quitout());
        }

        private void OnAppStart()
        {
            _isEnableHotkeysEnabled = SettingsManager.Default.EnableHotkeys;
            if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
            else _hotkeyManager.Stop();

            OnPropertyChanged(nameof(IsEnableHotkeysEnabled));
            IsAlwaysOnTopEnabled = SettingsManager.Default.AlwaysOnTop;
            _isFastQuitoutEnabled = SettingsManager.Default.FastQuitout;
            OnPropertyChanged(nameof(IsFastQuitoutEnabled));
            _isBabyJumpFixedEnabled = SettingsManager.Default.BabyJump;
            OnPropertyChanged(nameof(IsBabyJumpFixEnabled));
            _isDisableDoubleClickEnabled = SettingsManager.Default.DoubleClick;
            OnPropertyChanged(nameof(IsDisableDoubleClickEnabled));
        }

        private void OnGameLoaded()
        {
            _isLoaded = true;
            if (IsFastQuitoutEnabled) _settingsService.ToggleFastQuitout(true);
            if (IsBabyJumpFixEnabled) _settingsService.ToggleBabyJumpFix(true);
            if (IsDisableDoubleClickEnabled) _settingsService.ToggleDoubleClick(true);
        }

        private void LoadHotkeyDisplays()
        {
            foreach (var hotkey in _hotkeyLookup.Values)
            {
                hotkey.HotkeyText = GetHotkeyDisplayText(hotkey.ActionId);
            }
        }

        private string GetHotkeyDisplayText(string actionId)
        {
            Keys keys = _hotkeyManager.GetHotkey(actionId);
            return keys != null && keys.Values.ToArray().Length > 0 ? string.Join(" + ", keys) : "None";
        }

        private void TempHook_Down(object sender, KeyboardEventArgs e)
        {
            if (_currentSettingHotkeyId == null || e.Keys.IsEmpty)
                return;

            try
            {
                bool containsEnter = e.Keys.Values.Contains(Key.Enter) || e.Keys.Values.Contains(Key.Return);

                if (containsEnter && _currentKeys != null)
                {
                    _hotkeyManager.SetHotkey(_currentSettingHotkeyId, _currentKeys);
                    StopSettingHotkey();
                    e.IsHandled = true;
                    return;
                }

                if (e.Keys.Values.Contains(Key.Escape))
                {
                    CancelSettingHotkey();
                    e.IsHandled = true;
                    return;
                }

                if (containsEnter)
                {
                    e.IsHandled = true;
                    return;
                }

                if (e.Keys.IsEmpty)
                    return;

                _currentKeys = e.Keys;

                if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
                {
                    binding.HotkeyText = e.Keys.ToString();
                }
            }
            catch (Exception ex)
            {
                if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
                {
                    binding.HotkeyText = "Error: Invalid key combination";
                }
            }

            e.IsHandled = true;
        }

        private void StopSettingHotkey()
        {
            var hook = _tempHook;
            _tempHook = null;
            _currentSettingHotkeyId = null;
            _currentKeys = null;

            if (hook != null)
            {
                hook.Down -= TempHook_Down;
                try
                {
                    hook.Dispose();
                }
                catch (COMException)
                {
                    // Already stopped - harmless
                }
            }
        }

        private void HandleExistingHotkey(Keys currentKeys)
        {
            string existingHotkeyId = _hotkeyManager.GetActionIdByKeys(currentKeys);
            if (string.IsNullOrEmpty(existingHotkeyId)) return;

            _hotkeyManager.ClearHotkey(existingHotkeyId);
            if (_hotkeyLookup.TryGetValue(existingHotkeyId, out var binding))
            {
                binding.HotkeyText = "None";
            }
        }

        private void SetNewHotkey(string currentSettingHotkeyId, Keys currentKeys)
        {
            _hotkeyManager.SetHotkey(currentSettingHotkeyId, currentKeys);

            if (_hotkeyLookup.TryGetValue(currentSettingHotkeyId, out var binding))
            {
                binding.HotkeyText = new Keys(currentKeys.Values.ToArray()).ToString();
            }
        }

        #endregion

        public void ResetAttached() => _isLoaded = false;
    }
}