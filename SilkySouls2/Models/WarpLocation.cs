namespace SilkySouls2.Models
{
    public class WarpLocation
    {
        public int BonfireId { get; set; }
        public string MainArea { get; set; }
        public string LocationName { get; set; }
        public float[] Coordinates { get; set; }
        public float[] Angle { get; set; }

        public bool HasCoordinates => Coordinates != null && Coordinates.Length == 16;
    }
}