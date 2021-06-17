using UnityEngine;

namespace Mechanics {
    public class KnockbackData {
        public readonly Vector2 Direction;
        public readonly float Force = 2f;
        public readonly float Duration = 2f;
        public readonly int Damage = 0;
        public readonly GameObject KnockbackVFX;

        public KnockbackData(Vector2 direction, float force, float duration,
            int damage, GameObject knockbackVFX) {
            Direction = direction;
            Force = force;
            Duration = duration;
            Damage = damage;
            KnockbackVFX = knockbackVFX;
        }
    }
}