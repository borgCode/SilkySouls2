// 

using System.Numerics;

namespace SilkySouls2.Interfaces;

public interface IChrCtrlService
{
    int GetHp(nint chrCtrl);
    int GetMaxHp(nint chrCtrl);
    void SetHp(nint chrCtrl, int health);
    Vector3 GetPos(nint chrCtrl);
    void SetSpeed(nint chrCtrl, float value);
    float GetSpeed(nint chrCtrl);
}