//

using System.Numerics;

namespace SilkySouls2.Models;

public class Position(Vector4 coords, Vector4 orientation, uint mapId = 0)
{
    
    public Vector4 Coords { get; set; } = coords;
    public Vector4 Orientation { get; set; } = orientation;
    
    public Vector4 WarpCoords { get; set; }
    public Vector4 WarpQuaternion { get; set; }

    public uint MapId { get; set; } = mapId;
}