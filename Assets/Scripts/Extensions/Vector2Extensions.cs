using UnityEngine;

namespace Extensions {
    public static class Vector2Extensions {
        public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.x, v.y, 0);
    }
}