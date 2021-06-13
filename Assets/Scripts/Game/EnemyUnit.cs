using Mechanics.Health;
using Messages;
using UniRx;
using UnityEngine;

namespace Game {
    public class EnemyUnit : MonoBehaviour, IDamagable {
        [SerializeField] int _deathScoreAmount;
        [SerializeField] int _damageScoreAmount;
        [SerializeField] private HealthConfigSO _healthConfig;
        public HealthSystem HealthSystem { get; private set; }
        public GameObject Behaviour => gameObject;
        [SerializeField] private AudioClip _hurtSFX;
        [SerializeField] private AudioClip _deathSFX;

        public void OnHealthChanged(float prevAmount) {
            GameManager._instance.IncrementScore(HealthSystem.IsDead ? _deathScoreAmount : _damageScoreAmount);
            MessageBroker.Default.Publish(new PlaySFXEvent(HealthSystem.IsDead ? _deathSFX : _hurtSFX));
        }
        
        public void OnDeath() {
            Destroy(gameObject);
        }
        
        private void Start() {
            HealthSystem = new HealthSystem(this, _healthConfig.Config);
        }
    }
}