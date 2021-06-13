using UnityEngine;

namespace Messages {
    public class PlaySFXEvent {
        public readonly AudioClip Clip;
        public PlaySFXEvent(AudioClip clip) {
            Clip = clip;
        }
    }
}