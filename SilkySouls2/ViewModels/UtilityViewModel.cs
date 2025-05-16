using System;
using SilkySouls2.Memory;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class UtilityViewModel: BaseViewModel
    {
        
        private bool _isIvorySkipEnabled;
        private bool _isCreditSkipEnabled;
        private bool _is100DropEnabled;
        private bool _hitboxTest;
        private bool _isDrawEventEnabled;
        private bool _isDrawSoundEnabled;
        
        private bool _areButtonsEnabled;
        private readonly HotkeyManager _hotkeyManager;
        private readonly UtilityService _utilityService;
        private readonly PlayerViewModel _playerViewModel;
        
        private float _gameSpeed;
        
        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;
        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;
        private bool _isNoClipEnabled;
        private bool _wasNoDeathEnabled;
        
        public UtilityViewModel(UtilityService utilityService, HotkeyManager hotkeyManager, PlayerViewModel playerViewModel)
        {
            _playerViewModel = playerViewModel;
            _utilityService = utilityService;
            _hotkeyManager = hotkeyManager;
           
            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("ForceSave", () =>
            {
                if (!AreButtonsEnabled) return;
                _ = _utilityService.ForceSave();
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
            _hotkeyManager.RegisterAction("IncreaseGameSpeed", () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction("DecreaseGameSpeed", () => SetSpeed(Math.Max(0, GameSpeed - 0.50f)));
        }
        
        
        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set => SetProperty(ref _areButtonsEnabled, value);
        }
        
        public bool IsDrawHitboxEnabled
        {
            get => _hitboxTest;
            set
            {
                if (!SetProperty(ref _hitboxTest, value)) return;
                _utilityService.ToggleDrawHitbox(_hitboxTest);
            }
        }
        
        public bool IsDrawEventEnabled
        {
            get => _isDrawEventEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventEnabled, value)) return;
                _utilityService.ToggleDrawEvent(_isDrawEventEnabled);
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
        
        public bool IsCreditSkipEnabled
        {
            get => _isCreditSkipEnabled;
            set
            {
                if (!SetProperty(ref _isCreditSkipEnabled, value)) return;
                _utilityService.ToggleCreditSkip(_isCreditSkipEnabled);
            }
        }
        
        
        public bool IsIvorySkipEnabled
        {
            get => _isIvorySkipEnabled;
            set
            {
                if (!SetProperty(ref _isIvorySkipEnabled, value)) return;
                if (_isIvorySkipEnabled) _utilityService.SetMultipleEventOn(GameIds.EventFlags.IvoryBlackKnights);
                else _utilityService.SetMultipleEventOff(GameIds.EventFlags.IvoryBlackKnights);
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
                    // _utilityService.SetGameSpeed(value);
                }
            }
        }
        
        public void SetSpeed(float value) => GameSpeed = value;
        
        public void TryEnableFeatures()
        {
            if (IsIvorySkipEnabled) _utilityService.SetMultipleEventOn(GameIds.EventFlags.IvoryBlackKnights);
            if (IsCreditSkipEnabled) _utilityService.ToggleCreditSkip(true);
            // if (IsHitboxEnabled) _utilityService.ToggleHitboxView(true);
            // if (IsSoundViewEnabled) _utilityService.ToggleSoundView(true);
            // if (IsDrawEventEnabled) _utilityService.ToggleEventDraw(true);
            // if (IsTargetingViewEnabled)
            // {
            //     _utilityService.PatchDebugDraw(true);
            //     _utilityService.ToggleTargetingView(true);
            // }
            // if (IsHideMapEnabled) _utilityService.ToggleGroupMask(GroupMask.Map,true);
            // if (IsHideObjectsEnabled) _utilityService.ToggleGroupMask(GroupMask.Obj,true);
            // if (IsHideCharactersEnabled) _utilityService.ToggleGroupMask(GroupMask.Chr,true);
            // if (IsHideSfxEnabled) _utilityService.ToggleGroupMask(GroupMask.Sfx,true);
            // if (IsDisableEventEnabled) _utilityService.ToggleDisableEvent(true);
            // if (IsDrawLowHitEnabled)
            // {
            //     IsHideMapEnabled = true;
            //     _utilityService.ToggleHitIns(HitIns.LowHit, true);
            // }
            //
            // if (IsDrawHighHitEnabled)
            // {
            //     IsHideMapEnabled = true;
            //     _utilityService.ToggleHitIns(HitIns.HighHit, true);
            // }
            //
            // if (IsDrawChrRagdollEnabled)
            // {
            //     _utilityService.PatchDebugDraw(true);
            //     _utilityService.ToggleHitIns(HitIns.ChrRagdoll, true);
            // }
            // GameSpeed = _utilityService.GetGameSpeed();
            // CameraFov = _utilityService.GetCameraFov();
            AreButtonsEnabled = true;
        }
        
        public void DisableFeatures()
        {
            
            IsNoClipEnabled = false;
            AreButtonsEnabled = false;
        }

        public void ForceSave() => _ = _utilityService.ForceSave();

        public void TryApplyOneTimeFeatures()
        {
            if (Is100DropEnabled) _utilityService.Toggle100Drop(true);
        }

        public void Inject() => _utilityService.Inject();
    }
}