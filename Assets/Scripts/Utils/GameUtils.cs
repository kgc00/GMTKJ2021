using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils {
    public static class GameUtils {
            static Plane plane = new Plane(Vector3.forward, 0f);

            static Camera cam = Camera.main;

            public static Vector3 MousePosToWorldPosition(Vector2 mousePos)
            {
                if (cam == null) cam = Camera.main;
            
                if (cam != null)
                {
                    Ray ray = cam.ScreenPointToRay(mousePos);

                    if (plane.Raycast(ray, out float distanceToPlane))
                    {
                        var mouseLocation = ray.GetPoint(distanceToPlane);
                        mouseLocation.z = 0;
                        return mouseLocation;
                    }
                }

                return Vector3.zero;
            }
    }
}