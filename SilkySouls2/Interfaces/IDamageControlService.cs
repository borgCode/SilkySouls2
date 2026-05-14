// 

namespace SilkySouls2.Interfaces
{
    public interface IDamageControlService
    {
        bool IsOneShotEnabled { get; }
        bool IsDealNoDamageEnabled { get; }
        bool IsTargetHpFrozenEnabled { get; }
        void ToggleOneShot(bool enabled);
        void ToggleDealNoDamage(bool enabled);
        void ToggleFreezeTargetHp(bool enabled);
    }
}