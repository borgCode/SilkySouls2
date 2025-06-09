﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using H.Hooks;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region property setters
        private bool _isEnableHotkeysEnabled;
        private bool _isFastQuitoutEnabled;
        private bool _isBabyJumpFixedEnabled;
        private bool _isAlwaysOnTopEnabled;
        private bool _isLoaded;
        
        private string _savePos1HotkeyText;
        private string _savePos2HotkeyText;
        private string _restorePos1HotkeyText;
        private string _restorePos2HotkeyText;
        private string _rtsrHotkeyText;
        private string _noDeathHotkeyText;
        private string _oneShotHotkeyText;
        private string _noDamageHotkeyText;
        private string _togglePlayerSpeedHotkeyText;
        private string _increasePlayerSpeedHotkeyText;
        private string _decreasePlayerSpeedHotkeyText;
        private string _dealNoDamageHotkeyText;
        private string _restoreSpellcastsHotkeyText;
        private string _restoreHumanityHotkeyText;
        private string _restHotkeyText;
        
        
        private string _warpHotkeyText;
        private string _noClipHotkeyText;
        private string _enableFreeCamHotkeyText;
        private string _moveCamToPlayerHotkeyText;
        private string _enableDeathCamHotkeyText;
        
        private string _increaseGameSpeedHotkeyText;
        private string _decreaseGameSpeedHotkeyText;
        private string _increaseNoClipSpeedHotkeyText;
        private string _decreaseNoClipSpeedHotkeyText;
        
        private string _disableTargetAiHotkeyText;
        private string _freezeHpHotkeyText;
        private string _killTargetHotkeyText;
        private string _increaseTargetSpeedHotkeyText;
        private string _decreaseTargetSpeedHotkeyText;
        private string _enableTargetOptionsHotkeyText;
        private string _showAllResistancesHotkeyText;
        private string _targetRepeatActHotkeyText;
        private string _targetViewHotkeyText;
        
        private string _disableAiHotkeyText;
        private string _allNoDeathHotkeyText;
        private string _allNoDamageHotkeyText;
        private string _allRepeatActHotkeyText;
        

        private string _quitoutHotkeyText;
        private string _forceSaveHotkeyText;

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

        public string SavePos1HotkeyText
        {
            get => _savePos1HotkeyText;
            set => SetProperty(ref _savePos1HotkeyText, value);
        }

        public string SavePos2HotkeyText
        {
            get => _savePos2HotkeyText;
            set => SetProperty(ref _savePos2HotkeyText, value);
        }

        public string RestorePos1HotkeyText
        {
            get => _restorePos1HotkeyText;
            set => SetProperty(ref _restorePos1HotkeyText, value);
        }

        public string RestorePos2HotkeyText
        {
            get => _restorePos2HotkeyText;
            set => SetProperty(ref _restorePos2HotkeyText, value);
        }

        public string RtsrHotkeyText
        {
            get => _rtsrHotkeyText;
            set => SetProperty(ref _rtsrHotkeyText, value);
        }

        public string NoDeathHotkeyText
        {
            get => _noDeathHotkeyText;
            set => SetProperty(ref _noDeathHotkeyText, value);
        }

        public string OneShotHotkeyText
        {
            get => _oneShotHotkeyText;
            set => SetProperty(ref _oneShotHotkeyText, value);
        }
        
        public string NoDamagePlayerHotkeyText
        {
            get => _noDamageHotkeyText;
            set => SetProperty(ref _noDamageHotkeyText, value);
        }
        
        public string TogglePlayerSpeedHotkeyText
        {
            get => _togglePlayerSpeedHotkeyText;
            set => SetProperty(ref _togglePlayerSpeedHotkeyText, value);
        }
        public string IncreasePlayerSpeedHotkeyText
        {
            get => _increasePlayerSpeedHotkeyText;
            set => SetProperty(ref _increasePlayerSpeedHotkeyText, value);
        }

        public string DecreasePlayerSpeedHotkeyText
        {
            get => _decreasePlayerSpeedHotkeyText;
            set => SetProperty(ref _decreasePlayerSpeedHotkeyText, value);
        }
        
        
        public string DealNoDamageHotkeyText
        {
            get => _dealNoDamageHotkeyText;
            set => SetProperty(ref _dealNoDamageHotkeyText, value);
        }
        
        public string RestoreSpellcastsHotkeyText
        {
            get => _restoreSpellcastsHotkeyText;
            set => SetProperty(ref _restoreSpellcastsHotkeyText, value);
        }
        
        public string RestoreHumanityHotkeyText
        {
            get => _restoreHumanityHotkeyText;
            set => SetProperty(ref _restoreHumanityHotkeyText, value);
        }
        
        public string RestHotkeyText
        {
            get => _restHotkeyText;
            set => SetProperty(ref _restHotkeyText, value);
        }
        
        public string IncreaseGameSpeedHotkeyText
        {
            get => _increaseGameSpeedHotkeyText;
            set => SetProperty(ref _increaseGameSpeedHotkeyText, value);
        }

        public string DecreaseGameSpeedHotkeyText
        {
            get => _decreaseGameSpeedHotkeyText;
            set => SetProperty(ref _decreaseGameSpeedHotkeyText, value);
        }
        
        public string IncreaseNoClipSpeedHotkeyText
        {
            get => _increaseNoClipSpeedHotkeyText;
            set => SetProperty(ref _increaseNoClipSpeedHotkeyText, value);
        }
        
        public string DecreaseNoClipSpeedHotkeyText
        {
            get => _decreaseNoClipSpeedHotkeyText;
            set => SetProperty(ref _decreaseNoClipSpeedHotkeyText, value);
        }
        
        public string WarpHotkeyText
        {
            get => _warpHotkeyText;
            set => SetProperty(ref _warpHotkeyText, value);
        }
        
        public string NoClipHotkeyText
        {
            get => _noClipHotkeyText;
            set => SetProperty(ref _noClipHotkeyText, value);
        }
        
        public string EnableFreeCamHotkeyText
        {
            get => _enableFreeCamHotkeyText;
            set => SetProperty(ref _enableFreeCamHotkeyText, value);
        }
        
        public string MoveCamToPlayerHotkeyText
        {
            get => _moveCamToPlayerHotkeyText;
            set => SetProperty(ref _moveCamToPlayerHotkeyText, value);
        }
        
        public string QuitoutHotkeyText
        {
            get => _quitoutHotkeyText;
            set => SetProperty(ref _quitoutHotkeyText, value);
        }
        
        public string ForceSaveHotkeyText
        {
            get => _forceSaveHotkeyText;
            set => SetProperty(ref _forceSaveHotkeyText, value);
        }
        
        public string EnableDeathCamHotkeyText
        {
            get => _enableDeathCamHotkeyText;
            set => SetProperty(ref _enableDeathCamHotkeyText, value);
        }
        
        
        public string DisableTargetAiHotkeyText
        {
            get => _disableTargetAiHotkeyText;
            set => SetProperty(ref _disableTargetAiHotkeyText, value);
        }
        
        public string FreezeHpHotkeyText
        {
            get => _freezeHpHotkeyText;
            set => SetProperty(ref _freezeHpHotkeyText, value);
        }
        
        public string KillTargetHotkeyText
        {
            get => _killTargetHotkeyText;
            set => SetProperty(ref _killTargetHotkeyText, value);
        }
        
        public string TargetViewHotkeyText
        {
            get => _targetViewHotkeyText;
            set => SetProperty(ref _targetViewHotkeyText, value);
        }
        
        public string TargetRepeatActHotkeyText
        {
            get => _targetRepeatActHotkeyText;
            set => SetProperty(ref _targetRepeatActHotkeyText, value);
        }
        
        public string IncreaseTargetSpeedHotkeyText
        {
            get => _increaseTargetSpeedHotkeyText;
            set => SetProperty(ref _increaseTargetSpeedHotkeyText, value);
        }
        
        public string DecreaseTargetSpeedHotkeyText
        {
            get => _decreaseTargetSpeedHotkeyText;
            set => SetProperty(ref _decreaseTargetSpeedHotkeyText, value);
        }
        
        public string EnableTargetOptionsHotkeyText
        {
            get => _enableTargetOptionsHotkeyText;
            set => SetProperty(ref _enableTargetOptionsHotkeyText, value);
        }
        
        public string ShowAllResistancesHotkeyText
        {
            get => _showAllResistancesHotkeyText;
            set => SetProperty(ref _showAllResistancesHotkeyText, value);
        }
        
        public string DisableAiHotkeyText
        {
            get => _disableAiHotkeyText;
            set => SetProperty(ref _disableAiHotkeyText, value);
        }
        
        public string AllNoDeathHotkeyText
        {
            get => _allNoDeathHotkeyText;
            set => SetProperty(ref _allNoDeathHotkeyText, value);
        }
        
        public string AllNoDamageHotkeyText
        {
            get => _allNoDamageHotkeyText;
            set => SetProperty(ref _allNoDamageHotkeyText, value);
        }
        
        public string AllRepeatActHotkeyText
        {
            get => _allRepeatActHotkeyText;
            set => SetProperty(ref _allRepeatActHotkeyText, value);
        }
        
        
        #endregion
        
        private string _currentSettingHotkeyId;
        private LowLevelKeyboardHook _tempHook;
        private Keys _currentKeys;
        
        private readonly Dictionary<string, Action<string>> _propertySetters;
        
        private readonly SettingsService _settingsService;
        private readonly HotkeyManager _hotkeyManager;

        public SettingsViewModel(SettingsService settingsService, HotkeyManager hotkeyManager)
        {
            _settingsService = settingsService;
            _hotkeyManager = hotkeyManager;

            RegisterHotkeys();
            
            _propertySetters = new Dictionary<string, Action<string>>
            {
                { "SavePos1", text => SavePos1HotkeyText = text },
                { "SavePos2", text => SavePos2HotkeyText = text },
                { "RestorePos1", text => RestorePos1HotkeyText = text },
                { "RestorePos2", text => RestorePos2HotkeyText = text },
                { "RTSR", text => RtsrHotkeyText = text },
                { "NoDeath", text => NoDeathHotkeyText = text },
                { "OneShot", text => OneShotHotkeyText = text },
                { "PlayerNoDamage", text => NoDamagePlayerHotkeyText = text },
                { "TogglePlayerSpeed", text => TogglePlayerSpeedHotkeyText = text },
                { "IncreasePlayerSpeed", text => IncreasePlayerSpeedHotkeyText = text },
                { "DecreasePlayerSpeed", text => DecreasePlayerSpeedHotkeyText = text },
                { "DealNoDamage", text => DealNoDamageHotkeyText = text },
                { "RestoreSpellcasts", text => RestoreSpellcastsHotkeyText = text },
                { "RestoreHumanity", text => RestoreHumanityHotkeyText = text },
                { "Rest", text => RestHotkeyText = text },
                { "IncreaseGameSpeed", text => IncreaseGameSpeedHotkeyText = text },
                { "DecreaseGameSpeed", text => DecreaseGameSpeedHotkeyText = text },
                { "NoClip", text => NoClipHotkeyText = text },
                { "Warp", text => WarpHotkeyText = text },
                { "IncreaseNoClipSpeed", text => IncreaseNoClipSpeedHotkeyText = text },
                { "DecreaseNoClipSpeed", text => DecreaseNoClipSpeedHotkeyText = text },
                { "DisableTargetAi", text => DisableTargetAiHotkeyText = text },
                { "KillTarget", text => KillTargetHotkeyText = text },
                { "TargetView", text => TargetViewHotkeyText = text },
                { "FreezeHp", text => FreezeHpHotkeyText = text },
                { "TargetRepeatAct", text => TargetRepeatActHotkeyText = text },
                { "IncreaseTargetSpeed", text => IncreaseTargetSpeedHotkeyText = text },
                { "DecreaseTargetSpeed", text => DecreaseTargetSpeedHotkeyText = text },
                { "EnableTargetOptions", text => EnableTargetOptionsHotkeyText = text },
                { "ShowAllResistances", text => ShowAllResistancesHotkeyText = text },
                { "DisableAi", text => DisableAiHotkeyText = text },
                { "AllNoDeath", text => AllNoDeathHotkeyText = text },
                { "AllNoDamage", text => AllNoDamageHotkeyText = text },
                { "AllRepeatAct", text => AllRepeatActHotkeyText = text },
                { "Quitout", text => QuitoutHotkeyText = text },
                { "ForceSave", text => ForceSaveHotkeyText = text },
                { "EnableFreeCam", text => EnableFreeCamHotkeyText = text },
                { "MoveCamToPlayer", text => MoveCamToPlayerHotkeyText = text },
            };
            
            LoadHotkeyDisplays();
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("Quitout", () => _settingsService.Quitout());
        }

#region hotkeyStuff
        private void LoadHotkeyDisplays()
        {
            foreach (var entry in _propertySetters)
            {
                string actionId = entry.Key;
                Action<string> setter = entry.Value;

                setter(GetHotkeyDisplayText(actionId));
            }
        }
        
        private string GetHotkeyDisplayText(string actionId)
        {
            Keys keys = _hotkeyManager.GetHotkey(actionId);
            return keys != null && keys.Values.ToArray().Length > 0 ? string.Join(" + ", keys) : "None";
        }
        
        public void StartSettingHotkey(string actionId)
        {
            if (_currentSettingHotkeyId != null &&
                _propertySetters.TryGetValue(_currentSettingHotkeyId, out var prevSetter))
            {
                prevSetter(GetHotkeyDisplayText(_currentSettingHotkeyId));
            }

            _currentSettingHotkeyId = actionId;

            if (_propertySetters.TryGetValue(actionId, out var setter))
            {
                setter("Press keys...");
            }

            _tempHook = new LowLevelKeyboardHook();
            _tempHook.IsExtendedMode = true;
            _tempHook.Down += TempHook_Down;
            _tempHook.Start();
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

                if (_propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
                {
                    string keyText = e.Keys.ToString();
                    setter(keyText);
                }
            }
            catch (Exception ex)
            {
                if (_propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
                {
                    setter("Error: Invalid key combination");
                }
            }

            e.IsHandled = true;
        }

        private void StopSettingHotkey()
        {
            if (_tempHook != null)
            {
                _tempHook.Down -= TempHook_Down;
                _tempHook.Dispose();
                _tempHook = null;
            }

            _currentSettingHotkeyId = null;
            _currentKeys = null;
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

        private void HandleExistingHotkey(Keys currentKeys)
        {
            string existingHotkeyId = _hotkeyManager.GetActionIdByKeys(currentKeys);
            if (string.IsNullOrEmpty(existingHotkeyId)) return;
            
            _hotkeyManager.ClearHotkey(existingHotkeyId);
            if (_propertySetters.TryGetValue(existingHotkeyId, out var oldSetter))
            {
                oldSetter("None");
            }
        }
        
        private void SetNewHotkey(string currentSettingHotkeyId, Keys currentKeys)
        {
            _hotkeyManager.SetHotkey(currentSettingHotkeyId, currentKeys);
            
            if (_propertySetters.TryGetValue(currentSettingHotkeyId, out var setter))
            {
                setter(new Keys(currentKeys.Values.ToArray()).ToString());
            }
        }
        
        public void CancelSettingHotkey()
        {
            if (_currentSettingHotkeyId != null &&
                _propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
            {
                setter("None");
                _hotkeyManager.SetHotkey(_currentSettingHotkeyId, new Keys());
            }

            StopSettingHotkey();
        }
        #endregion
        
        
        public void ApplyLoadedOptions()
        {
            _isLoaded = true;
            if (IsFastQuitoutEnabled) _settingsService.ToggleFastQuitout(true);
            if (IsBabyJumpFixEnabled) _settingsService.ToggleBabyJumpFix(true);
        }
        
        
        public void ApplyStartUpOptions()
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
        }

        public void ResetAttached() => _isLoaded = false;
    }
}