using UnityEngine;
namespace Messages {
    public class VFXEvent {
        public readonly GameObject VFXObject;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public VFXEvent(GameObject vfxPrefab, Vector3 pos, Quaternion? rot = null) {
            VFXObject = vfxPrefab;
            Position = pos;
            Rotation = rot.HasValue ? rot.Value : Quaternion.identity;
        }
    }
}