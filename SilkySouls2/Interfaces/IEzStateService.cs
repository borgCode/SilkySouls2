// 

using SilkySouls2.GameIds;

namespace SilkySouls2.Interfaces
{
    public interface IEzStateService
    {
        void ExecuteEventFromGameThread(EzState.EventCommand command, int areaId = 0, int areaIndex = 0);
        void ExecuteEvent(EzState.EventCommand command, int areaId = 0, int areaIndex = 0);
    }
}