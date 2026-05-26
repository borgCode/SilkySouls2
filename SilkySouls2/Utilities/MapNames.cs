namespace SilkySouls2.Utilities;

public static class MapNames
{
    public static string ToMapName(uint id) =>
        $"m{(byte)(id >> 24):D2}_{(byte)(id >> 16):D2}_{(byte)(id >> 8):D2}_{(byte)id:D2}";
}
