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
        
        private bool _areButtonsEnabled;
        private readonly HotkeyManager _hotkeyManager;
        private readonly UtilityService _utilityService;
        
        private const float DefaultNoclipMultiplier = 0.25f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;
        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;
        private bool _isNoClipEnabled;
        
        public UtilityViewModel(UtilityService utilityService, HotkeyManager hotkeyManager)
        {

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
        }
        
        
        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set => SetProperty(ref _areButtonsEnabled, value);
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
                    // IsFreeCamEnabled = false;
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                    // _wasNoDeathEnabled = _playerViewModel.IsNoDeathEnabled;
                    // _playerViewModel.IsNoDeathEnabled = true;
                    // _playerViewModel.IsSilentEnabled = true;
                    // _playerViewModel.IsInvisibleEnabled = true;
                }
                else
                {
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                    // _playerViewModel.IsNoDeathEnabled = _wasNoDeathEnabled;
                    // _playerViewModel.IsSilentEnabled = false;
                    // _playerViewModel.IsInvisibleEnabled = false;
                    // NoClipSpeed = DefaultNoclipMultiplier;
                }
            }
        }
        
        
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

        public void ForceSave() => _ = _utilityService.ForceSave();

        public void TryApplyOneTimeFeatures()
        {
            if (Is100DropEnabled) _utilityService.Toggle100Drop(true);
        }
    }
}