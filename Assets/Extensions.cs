using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    public static class Vector3Extension
    {
        public static float2 ToFloat2(this Vector3 vector3)
            => new float2(vector3.x, vector3.y);
    }

    public static class Float2Extension
    {
        public static Vector3 ToVector3(this float2 vector3)
            => new Vector3(vector3.x, vector3.y);
    }
}

