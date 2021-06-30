using Mechanics.Health;
using Messages;
using UniRx;
using UnityEngine;

namespace Game {
    public class PlayerUnit : MonoBehaviour, IDamagable {
        [SerializeField] private VFXService SORef; 
        [SerializeField] private HealthConfigSO _healthConfig;
        public HealthSystem HealthSystem { get; private set; }
        public GameObject Behaviour => gameObject;
        [SerializeField] private AudioClip _hurtSFX;
        [SerializeField] private AudioClip _deathSFX;

        public void OnHealthChanged(float prevAmount) {
            MessageBroker.Default.Publish(
                new PlayerHealthChanged(new HealthAdjustment(prevAmount, HealthSystem.CurrentHp, HealthSystem.MaxHp)));
            MessageBroker.Default.Publish(new PlaySFXEvent(HealthSystem.IsDead ? _deathSFX : _hurtSFX));
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