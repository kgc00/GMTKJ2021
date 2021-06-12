using UnityEngine;

namespace Messages {
    public class MoveActionPerformed {
        public readonly Vector2 Motion;
        public MoveActionPerformed(Vector2 motion) {
            Motion = motion;
        }
    }
}