using UnityEngine;

namespace Mechanics {
    public class KnockbackData {
        public readonly Vector2 Direction;
        public readonly float Force = 2f;
        public readonly float Duration = 2f;
        public readonly int Damage = 0;

        public KnockbackData(Vector2 direction, float force, float duration,
            int damage) {
            Direction = direction;
            Force = force;
            Duration = duration;
            Damage = damage;
        }
    }
}