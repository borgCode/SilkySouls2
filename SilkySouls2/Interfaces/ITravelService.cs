using SilkySouls2.Models.V2;

namespace SilkySouls2.Interfaces
{
    public interface ITravelService
    {
        void Warp(WarpEntry entry);
        void UnlockAllBonfires();
    }
}
