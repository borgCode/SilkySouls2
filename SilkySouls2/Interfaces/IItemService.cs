// 

using SilkySouls2.Models;

namespace SilkySouls2.Interfaces
{
    public interface IItemService
    {
        void SpawnItem(Item selectedItem, int selectedUpgrade, int selectedQuantity, int selectedInfusion,
            float durability = 0.0f);

        void SetAutoSpawnWeapon(int wepId);
        void Reset();
        void SignalClose();
    }
}