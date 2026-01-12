// 

using System.Numerics;
using SilkySouls2.Interfaces;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class ChrCtrlService(IMemoryService memoryService) : IChrCtrlService
{
    public int GetHp(nint chrCtrl) => memoryService.Read<int>(chrCtrl + GameManagerImp.ChrCtrlOffsets.Hp);
    public int GetMaxHp(nint chrCtrl) => memoryService.Read<int>(chrCtrl + GameManagerImp.ChrCtrlOffsets.MaxHp);

    public void SetHp(nint chrCtrl, int health) =>
        memoryService.Write(chrCtrl + GameManagerImp.ChrCtrlOffsets.Hp, health);

    public Vector3 GetPos(nint chrCtrl)
    {
        var v = memoryService.Read<Vector3>(chrCtrl + GameManagerImp.ChrCtrlOffsets.Coords);
        return new Vector3(v.X, v.Z, v.Y);
    }

    public void SetSpeed(nint chrCtrl, float value) =>
        memoryService.Write(chrCtrl + GameManagerImp.ChrCtrlOffsets.Speed, value);

    public float GetSpeed(nint chrCtrl) => memoryService.Read<float>(chrCtrl + GameManagerImp.ChrCtrlOffsets.Speed);
}