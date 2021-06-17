
using UniRx;
using Messages;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "VFXService", menuName = "Game/VFXService", order = 0)]
public class VFXService : ScriptableObject {

    private void OnEnable() {
        MessageBroker.Default.Receive<VFXEvent>().Where(x => x.Transform != null).Subscribe(HandleVFXEventTransform);
        MessageBroker.Default.Receive<VFXEvent>().Where(x => x.Transform == null).Subscribe(HandleVFXEvent);
    }

    private void HandleVFXEventTransform(VFXEvent vfxEvent) {
        var go = Instantiate(vfxEvent.VFXObject, vfxEvent.Transform);
        go.transform.localRotation = Quaternion.identity;
    }

    private void HandleVFXEvent(VFXEvent vfxEvent) {
        Instantiate(vfxEvent.VFXObject, vfxEvent.Position, vfxEvent.Rotation);
    }
}
