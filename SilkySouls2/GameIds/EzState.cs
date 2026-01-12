// 

namespace SilkySouls2.GameIds
{
    public class EzState
    {
        public struct EventCommand(int commandId, params int[] parameters)
        {
            public int CommandId { get; } = commandId;
            public int[] Params { get; } = parameters;
        }

        public static class EventCommands
        {
            public static EventCommand DisableWhiteDoorKeyGuide(int objId, int targetState) =>
                new(131622, objId, targetState);

            public static EventCommand ChangeObjState(int objId, int targetState) =>
                new(131636, objId, targetState);

            public static EventCommand DeleteNavimeshAttribute(int eventId, int targetState) =>
                new(132132, eventId, targetState);

            public static readonly EventCommand OpenCharacterCreationMenu = new(130451);
        }

        public struct ChrEventCommand(int commandId, params int[] parameters)
        {
            public int CommandId { get; } = commandId;
            public int[] Params { get; } = parameters;
        }
    }
}