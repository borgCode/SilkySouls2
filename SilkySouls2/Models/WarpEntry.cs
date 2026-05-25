using System.Text.Json.Serialization;

namespace SilkySouls2.Models
{
    public class WarpEntry
    {
        [JsonPropertyName("kind")]        public WarpKind Kind { get; set; }
        [JsonPropertyName("area")]        public string Area { get; set; }
        [JsonPropertyName("name")]        public string Name { get; set; }

        [JsonPropertyName("bonfireId")]   public int? BonfireId { get; set; }
        [JsonPropertyName("mapId")]       public uint? MapId { get; set; }
        [JsonPropertyName("eventPointId")]public int? EventPointId { get; set; }
        [JsonPropertyName("pos")]         public float[] Pos { get; set; }
        [JsonPropertyName("quat")]        public float[] Quat { get; set; }
    }
}
