using UnityEngine;

namespace Mechanics.Health {
    public interface IDamagable {
        HealthSystem HealthSystem { get; }
        GameObject Behaviour { get; }


        void OnHealthChanged(float prevAmount);
        void OnDeath();
    }
}