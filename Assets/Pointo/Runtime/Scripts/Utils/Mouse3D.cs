using UnityEngine;

namespace Berserk.Utils.Mouse
{
    public class Mouse3D
    {
        public static Vector3 GetMouseWorldPosition(LayerMask maskToPoint)
        {
            Camera cam = Camera.main;
            return GetMouseWorldPosition(cam, maskToPoint);
        }
    
        public static Vector3 GetMouseWorldPosition(Camera cam, LayerMask maskToPoint)
        {
            if (cam == null) return Vector3.zero;
        
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit, 999f, maskToPoint) ? hit.point : Vector3.zero;
        }

        public static GameObject GetMouseWorldObject(LayerMask maskToPoint)
        {
            Camera cam = Camera.main;
            return GetMouseWorldObject(cam, maskToPoint);
        }
        
        public static GameObject GetMouseWorldObject(Camera cam, LayerMask maskToPoint)
        {
            if (cam == null) return null;
        
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit, 999f, maskToPoint) ? hit.transform.gameObject : null;
        }
    }

}