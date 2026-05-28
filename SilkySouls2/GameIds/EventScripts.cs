using System.Collections.Generic;
using System.Linq;
using static SilkySouls2.GameIds.EzState.EventCommands;

namespace SilkySouls2.GameIds
{
    public readonly struct ScriptStep(EzState.EventCommand command, int area)
    {
        public EzState.EventCommand Command { get; } = command;
        public int Area { get; } = area;
    }

    public static class EventScripts
    {
        public static List<ScriptStep> OpenGargsDoor() => new()
        {
            new(ObjCtrl.ChangeObjState(ObjectMapEntity.GargoylesDoor.Id, ObjectMapEntity.GargoylesDoor.State), Area.Bastille),
            new(PointCtrl.DeleteNavimeshAttribute(MapGeneralLocation.GargoylesDoor.EventPointId, MapGeneralLocation.GargoylesDoor.Attribute), Area.Bastille),
            new(ObjCtrl.DisableWhiteDoorKeyGuide(ObjectMapEntity.GargoylesFogDoor.Id, ObjectMapEntity.GargoylesFogDoor.State), Area.Bastille),
        };

        public static List<ScriptStep> LightSinner() =>
            ObjectMapEntity.SinnerLightings
                .Select(l => new ScriptStep(ObjCtrl.ChangeObjState(l.Id, l.State), Area.Bastille))
                .ToList();

        public static List<ScriptStep> MoveFlexileShip()
        {
            var steps = new List<ScriptStep>
            {
                new(BaseCtrl.SetEventFlag(118000010, 1), Area.Wharf),
                new(ObjCtrl.AttachObjToObj(10182000, 150, 10182002), Area.Wharf),
                new(ObjCtrl.ChangeObjState(10182002, 70), Area.Wharf),
                new(ObjCtrl.SetMapPartDisplay(1, 1), Area.Wharf),

                new(ObjCtrl.SetHitEnabled(4, 1), Area.Wharf),
                new(ObjCtrl.SetHitEnabled(3, 1), Area.Wharf),
                new(ObjCtrl.ChangeObjState(10182000, 21), Area.Wharf),

                new(ObjCtrl.SetPointLightEnabled(10180030, 1, 0), Area.Wharf),
                new(ObjCtrl.SetPointLightEnabled(10180040, 1, 0), Area.Wharf),

                new(ObjCtrl.PlaySfxAtPoint(1000), Area.Wharf),
                new(ObjCtrl.PlaySfxAtPoint(1010), Area.Wharf),
                new(PointCtrl.DeleteNavimeshAttribute(100000, 2), Area.Wharf),
            };

            for (int id = 10182200; id <= 10182208; id++)
                steps.Add(new ScriptStep(ObjCtrl.ChangeObjState(id, 10), Area.Wharf));

            return steps;
        }
    }
}
