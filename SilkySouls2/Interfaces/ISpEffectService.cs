using SilkySouls2.GameIds;

namespace SilkySouls2.Interfaces;

public interface ISpEffectService
{
    void ApplySpEffect(nint chrCtrl, SpEffect spEffect);
}