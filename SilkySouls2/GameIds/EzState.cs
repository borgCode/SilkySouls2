namespace SilkySouls2.GameIds
{
    public static class EzState
    {
        public enum CtrlType
        {
            NonOwner = 0,
            Obj = 1,
            Chr = 2,
            EventPoint = 3,
        }

        public abstract class EventCommand(int commandId, int[] parameters)
        {
            public int CommandId { get; } = commandId;
            public int[] Params { get; } = parameters ?? [];
            public abstract CtrlType Ctrl { get; }
        }

        public sealed class NonOwnerCommand(int commandId, params int[] parameters)
            : EventCommand(commandId, parameters)
        {
            public override CtrlType Ctrl => CtrlType.NonOwner;
        }

        public sealed class ObjCommand(int commandId, byte taskType, byte flagId, params int[] parameters)
            : EventCommand(commandId, parameters)
        {
            public byte TaskType { get; } = taskType;
            public byte FlagId { get; } = flagId;
            public override CtrlType Ctrl => CtrlType.Obj;
        }

        public sealed class ChrCommand(int commandId, params int[] parameters) : EventCommand(commandId, parameters)
        {
            public override CtrlType Ctrl => CtrlType.Chr;
        }

        public sealed class EventPointCommand(int commandId, params int[] parameters)
            : EventCommand(commandId, parameters)
        {
            public override CtrlType Ctrl => CtrlType.EventPoint;
        }

        public static class EventCommands
        {
            public static ObjCommand DisableWhiteDoorKeyGuide(int objId, int targetState, byte taskType, byte flagId) =>
                new ObjCommand(131622, taskType, flagId, objId, targetState);

            public static ChrCommand ChangeObjState(int objId, int stateId) =>
                new ChrCommand(131636, objId, stateId);

            public static ObjCommand DeleteNavimeshAttribute(int eventId, int targetState, byte taskType, byte flagId) =>
                new ObjCommand(132132, taskType, flagId, eventId, targetState);

            public static ObjCommand OpenCharacterCreationMenu(byte taskType, byte flagId) =>
                new ObjCommand(130451, taskType, flagId);
        }
    }
}
