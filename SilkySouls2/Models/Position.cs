// 

using System.Numerics;

namespace SilkySouls2.Models;

public class Position(Vector4 coords, Vector4 orientation)
{
    public Vector4 Coords { get; set; } = coords;
    public Vector4 Orientation { get; set; } = orientation;
}