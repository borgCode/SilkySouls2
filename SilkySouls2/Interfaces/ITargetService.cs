// 

namespace SilkySouls2.Interfaces
{
    public interface ITargetService
    {
        void ToggleTargetHook(bool isEnabled);
        int GetTargetHp();
        int GetTargetMaxHp();
        void SetTargetHp(int health);
        long GetTargetChrCtrl();
        float[] GetTargetPos();
        int GetLastAct();
        (bool PoisonToxic, bool Bleed) GetImmunities();
        float GetTargetResistance(int offset);
        void ToggleCurrentActHook(bool isEnabled);
        void ToggleRepeatAct(bool isRepeatActEnabled);
        void SetTargetSpeed(float value);
        float GetTargetSpeed();
        void ClearLockedTarget();
        void ToggleTargetAi(bool isDisableTargetAiEnabled);
        bool IsAiDisabled(long targetChrCtrl);
        void ClearDisableEntities();
        bool IsLightPoiseImmune();
        int GetChrParam(int chrParamOffset);
        float GetChrCommonParam(int chrCommonOffset);
    }
}