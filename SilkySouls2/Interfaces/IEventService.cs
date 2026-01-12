// 

namespace SilkySouls2.Interfaces
{
    public interface IEventService
    {
        void SetEvent(long eventId, bool setVal);
        void SetMultipleEventOn(long[] gameIds);
        bool GetEvent(long gameId);
    }
}