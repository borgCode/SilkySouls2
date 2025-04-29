using SilkySouls2.Memory;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class UtilityViewModel: BaseViewModel
    {
        
        private bool _isIvorySkipEnabled;
        private bool _isCreditSkipEnabled;
        
        private bool _areButtonsEnabled;
        private readonly HotkeyManager _hotkeyManager;
        private readonly UtilityService _utilityService;
        
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

        public void ForceSave() => _utilityService.ForceSave();
    }
}