using Mechanics.Health;
using UnityEngine;

namespace Game {
    public class Unit : MonoBehaviour, IDamagable {
        [SerializeField] private HealthConfigSO _healthConfig;
        public HealthSystem HealthSystem { get; private set; }
        public GameObject Behaviour => gameObject;
        public void OnHealthChanged(float prevAmount) { }
        public void OnDeath() { Destroy(gameObject);}
        private void Start() {
            HealthSystem = new HealthSystem(this, _healthConfig.Config);
        }
    }
}