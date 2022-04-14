using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridTools
{
    public static class Tools
    {
        public static Vector3 GetMouseWordPosition()
        {
            var vec = GetMouseWordPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWordPositionWithZ()
        {
            return GetMouseWordPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWordPositionWithZ(Camera worldCamera)
        {
            return GetMouseWordPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWordPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            return worldCamera.ScreenToWorldPoint(screenPosition);
        }
    }
}

