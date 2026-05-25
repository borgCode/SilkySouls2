using System.Runtime.InteropServices;

namespace SilkySouls2.Models
{
    
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public struct WarpRequest
    {
        [FieldOffset(0x00)] public uint Kind;
        [FieldOffset(0x04)] public uint TransitionMode;
        [FieldOffset(0x08)] public uint MapId;
        [FieldOffset(0x0C)] public int  Unk0C;
        [FieldOffset(0x10)] public uint PostWarpDemoId;
        [FieldOffset(0x14)] public uint SpawnAnim;

        [FieldOffset(0x18)] public float PosX;
        [FieldOffset(0x1C)] public float PosY;
        [FieldOffset(0x20)] public float PosZ;
        [FieldOffset(0x24)] public float PosW;
        [FieldOffset(0x28)] public float QuatX;
        [FieldOffset(0x2C)] public float QuatY;
        [FieldOffset(0x30)] public float QuatZ;
        [FieldOffset(0x34)] public float QuatW;

        [FieldOffset(0x18)] public int PayloadId; 

        [FieldOffset(0x38)] public uint PreWarpDemoId;
        [FieldOffset(0x3C)] public byte PostSubmitFlag;
        [FieldOffset(0x3D)] public byte PostSubmitSpecialFlag;
        [FieldOffset(0x3E)] public ushort Unk3E;

        public const uint DefaultTransitionMode = 6;
        public const uint DefaultSpawnAnim = 3;

        public static WarpRequest ForBonfire(int bonfireId, uint mapId) => new()
        {
            Kind = (uint)WarpKind.Bonfire,
            TransitionMode = DefaultTransitionMode,
            MapId = mapId,
            Unk0C = -1,
            PostWarpDemoId = 0,
            SpawnAnim = DefaultSpawnAnim,
            PayloadId = bonfireId
        };

        public static WarpRequest ForEventPoint(int eventPointId, uint mapId) => new()
        {
            Kind = (uint)WarpKind.EventPoint,
            TransitionMode = DefaultTransitionMode,
            MapId = mapId,
            Unk0C = -1,
            PostWarpDemoId = 0,
            SpawnAnim = DefaultSpawnAnim,
            PayloadId = eventPointId
        };
        
        public static WarpRequest ForDirect(WarpKind kind, uint mapId, float[] pos, float[] quat)
        {
            return new WarpRequest
            {
                Kind = (uint)kind,
                TransitionMode = DefaultTransitionMode,
                MapId = mapId,
                Unk0C = -1,
                PostWarpDemoId = 0,
                SpawnAnim = DefaultSpawnAnim,
                PosX = pos[0], PosY = pos[1], PosZ = pos[2], PosW = pos[3],
                QuatX = quat[0], QuatY = quat[1], QuatZ = quat[2], QuatW = quat[3]
            };
        }
    }
}
