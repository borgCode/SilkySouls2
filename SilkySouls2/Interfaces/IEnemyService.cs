// 

namespace SilkySouls2.Interfaces
{
    public interface IEnemyService
    {
        void ToggleForlornSpawn(bool isGuaranteedSpawnEnabled, int funcId = 0, int currentSelected = 0);
        void UpdateForlornIndex(int selectedForlornIndex);
        void ToggleDisableAi(bool isAllDisableAiEnabled);
        void ToggleElanaSummons(bool isElanaSummonsEnabled, int rngVal = 0);
    }
}