using System;
using System.Numerics;

namespace SilkySouls2.Utilities
{
    public static class MatrixMath
    {
        // Off-diagonal differences are flipped
        // (e.g. m12 - m21 instead of m21 - m12) — DS2 expects the conjugate.
        public static Vector4 MatrixToQuaternion(
            float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22)
        {
            float trace = m00 + m11 + m22;
            float qx, qy, qz, qw;

            if (trace > 0.0f)
            {
                float s = (float)Math.Sqrt(trace + 1.0f) * 2.0f;
                qw = 0.25f * s;
                qx = (m12 - m21) / s;
                qy = (m20 - m02) / s;
                qz = (m01 - m10) / s;
            }
            else if (m00 > m11 && m00 > m22)
            {
                float s = (float)Math.Sqrt(1.0f + m00 - m11 - m22) * 2.0f;
                qw = (m12 - m21) / s;
                qx = 0.25f * s;
                qy = (m01 + m10) / s;
                qz = (m02 + m20) / s;
            }
            else if (m11 > m22)
            {
                float s = (float)Math.Sqrt(1.0f + m11 - m00 - m22) * 2.0f;
                qw = (m20 - m02) / s;
                qx = (m01 + m10) / s;
                qy = 0.25f * s;
                qz = (m12 + m21) / s;
            }
            else
            {
                float s = (float)Math.Sqrt(1.0f + m22 - m00 - m11) * 2.0f;
                qw = (m01 - m10) / s;
                qx = (m02 + m20) / s;
                qy = (m12 + m21) / s;
                qz = 0.25f * s;
            }

            float len = (float)Math.Sqrt(qx * qx + qy * qy + qz * qz + qw * qw);
            if (len > 0.0f)
            {
                qx /= len;
                qy /= len;
                qz /= len;
                qw /= len;
            }

            return new Vector4(qx, qy, qz, qw);
        }
    }
}
