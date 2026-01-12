// 

using SilkySouls2.Models;

namespace SilkySouls2.Interfaces
{
    public interface ITravelService
    {
        void Warp(WarpLocation location, bool isRestOnWarpEnabled);
        void UnlockAllBonfires();
    }
}