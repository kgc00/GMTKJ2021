
using UniRx;
using Messages;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "VFXService", menuName = "Game/VFXService", order = 0)]
public class VFXService : ScriptableObject {

    private void OnEnable() {
        MessageBroker.Default.Receive<VFXEvent>().Subscribe(HandleVFXEvent);
    }

    private void HandleVFXEvent(VFXEvent vfxEvent) {
        Instantiate(vfxEvent.VFXObject, vfxEvent.Position, vfxEvent.Rotation);
    }
}
