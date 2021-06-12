using Mechanics.Health;
using Messages;
using UniRx;
using UnityEngine;

namespace Game {
    public class PlayerUnit : MonoBehaviour, IDamagable {
        [SerializeField] private HealthConfigSO _healthConfig;
        public HealthSystem HealthSystem { get; private set; }
        public GameObject Behaviour => gameObject;

        public void OnHealthChanged(float prevAmount) {
            MessageBroker.Default.Publish(
                new PlayerHealthChanged(new HealthAdjustment(prevAmount, HealthSystem.CurrentHp, HealthSystem.MaxHp)));
        }

        public void OnDeath() {
            Destroy(gameObject);
            GameManager._instance.ResetLevel();
        }

        private void Start() {
            HealthSystem = new HealthSystem(this, _healthConfig.Config);
        }
    }
}