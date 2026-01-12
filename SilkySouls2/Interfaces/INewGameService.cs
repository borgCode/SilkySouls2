// 

namespace SilkySouls2.Interfaces;

public interface INewGameService
{
    void RequestNewGameDetect();
    void ReleaseNewGameDetect();
    int GetCount();
    void Reset();

}