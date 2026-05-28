// 

namespace SilkySouls2.GameIds
{
    public class ObjectMapEntity
    {
        public int Id;
        public int State;
        
        public static readonly ObjectMapEntity SinnerLighting1 = new() { Id = 10161002, State = 71 };
        public static readonly ObjectMapEntity SinnerLighting2 = new() { Id = 10161000, State = 70 };
        public static readonly ObjectMapEntity SinnerLighting3 = new() { Id = 10161003, State = 71 };
        public static readonly ObjectMapEntity SinnerLighting4 = new() { Id = 10161001, State = 70 };

        public static readonly ObjectMapEntity[] SinnerLightings =
            { SinnerLighting1, SinnerLighting2, SinnerLighting3, SinnerLighting4 };
        
        
        public static readonly ObjectMapEntity GargoylesDoor = new() { Id = 10161051, State = 20 };
        public static readonly ObjectMapEntity GargoylesFogDoor = new() { Id = 10160620, State = 0 };
    }
}