// 

using System.Numerics;
using SilkySouls2.GameIds;

namespace SilkySouls2.Interfaces
{
    public interface IPlayerService
    {
        int GetHp();
        int GetMaxHp();
        void SetHp(int hp);
        void SetFullHp();
        void SetRtsr();
        int GetSp();
        void SetSp(int sp);
        void ToggleNoDeath(bool isNoDeathEnabled);
        void ToggleNoDamage(bool isNoDamageEnabled);
        void ToggleInfiniteStamina(bool isInfiniteStaminaEnabled);
        int GetPlayerStat(int statOffset);
        void SetPlayerStat(int statOffset, byte val);
        int GetSoulLevel();
        float GetPlayerSpeed();
        void SetPlayerSpeed(float speed);
        void ToggleNoGoodsConsume(bool isNoGoodsConsumeEnabled);
        void ToggleInfiniteCasts(bool isInfiniteCastsEnabled);
        void ToggleInfiniteDurability(bool isInfiniteDuraEnabled);
        void SavePos(int index);
        void RestorePos(int index);
        Vector3 GetCoords();
        void SetNewGame(int value);
        int GetNewGame();
        void GiveSouls(int souls);
        int GetSoulMemory();
        void RestoreSpellcasts();
        void ToggleSilent(bool isSilentEnabled);
        void ToggleHidden(bool isHiddenEnabled);
        void ToggleInfinitePoise(bool isInfinitePoiseEnabled);
        void SetSpEffect(SpEffect restoreHumanity);
        void ToggleNoSoulGain(bool isEnabled);
        void ToggleNoHollowing(bool isEnabled);
        void ToggleNoSoulLoss(bool isEnabled);
        void ToggleSoulMemWrite(bool isEnabled);
        


    }
}