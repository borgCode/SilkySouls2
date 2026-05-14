// 

namespace SilkySouls2.Interfaces
{
    public interface IEventService
    {
        void SetEvent(int eventId, bool setVal);
        void SetMultipleEventOn(int[] gameIds);
        bool GetEvent(int gameId);
    }
}