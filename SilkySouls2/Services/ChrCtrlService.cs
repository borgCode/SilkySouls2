// 

using System.Numerics;
using SilkySouls2.Interfaces;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class ChrCtrlService(IMemoryService memoryService) : IChrCtrlService
{
    public int GetHp(nint chrCtrl) => memoryService.Read<int>(chrCtrl + ChrCtrl.Hp);
    public int GetMaxHp(nint chrCtrl) => memoryService.Read<int>(chrCtrl + ChrCtrl.MaxHp);

    public void SetHp(nint chrCtrl, int health) =>
        memoryService.Write(chrCtrl + ChrCtrl.Hp, health);

    public Vector3 GetPos(nint chrCtrl)
    {
        var v = memoryService.Read<Vector3>(chrCtrl + ChrCtrl.Coords);
        return new Vector3(v.X, v.Z, v.Y);
    }

    public void SetSpeed(nint chrCtrl, float value) =>
        memoryService.Write(chrCtrl + ChrCtrl.Speed, value);

    public float GetSpeed(nint chrCtrl) => memoryService.Read<float>(chrCtrl + ChrCtrl.Speed);
}