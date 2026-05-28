//

namespace SilkySouls2.GameIds
{
    public static class EzState
    {
        public struct EventCommand(int commandId, params int[] parameters)
        {
            public int CommandId { get; } = commandId;
            public int[] Params { get; } = parameters;
        }

        public static class EventCommands
        {
            public static class BaseCtrl
            {
                public static EventCommand SetEventFlag(int eventFlagId, int setVal) =>
                    new(130101, eventFlagId, setVal);
            }
            
            public static class ObjCtrl
            {
                public static EventCommand ChangeObjState(int objId, int stateId) =>
                    new(131636, objId, stateId);

                public static EventCommand AttachObjToObj(int parentObjId, int parentDummyPolyId, int childObjId) =>
                    new(131641, parentObjId, parentDummyPolyId, childObjId);

                public static EventCommand SetMapPartDisplay(int partGroupId, int enabled) =>
                    new(132154, partGroupId, enabled);

                public static EventCommand SetHitEnabled(int id, int enabled) =>
                    new(132153, id, enabled);

                public static EventCommand SetPointLightEnabled(int eventLightId, int enabled, int fadeTime) =>
                    new(132101, eventLightId, enabled, fadeTime);

                public static EventCommand PlaySfxAtPoint(int sfxId) =>
                    new(131501, sfxId);

                public static EventCommand DisableWhiteDoorKeyGuide(int objId, int targetState) =>
                    new(131622, objId, targetState);
            }

            public static class PointCtrl
            {
                public static EventCommand DeleteNavimeshAttribute(int eventPointId, int attribute) =>
                    new(132132, eventPointId, attribute);
            }
        }

        public struct ChrEventCommand(int commandId, params int[] parameters)
        {
            public int CommandId { get; } = commandId;
            public int[] Params { get; } = parameters;
        }
    }
}
