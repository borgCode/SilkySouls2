// 

namespace SilkySouls2.Interfaces
{
    public interface ISettingsService
    {
        void Quitout();
        void ToggleFastQuitout(bool isEnabled);
        void ToggleBabyJumpFix(bool isEnabled);
        void ToggleDoubleClick(bool isDisableDoubleClickEnabled);
    }
}