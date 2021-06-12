using Mechanics.Health;
using UnityEngine;

namespace Game {
    public class EnemyUnit : MonoBehaviour, IDamagable {
        [SerializeField] int _deathScoreAmount;
        [SerializeField] int _damageScoreAmount;
        [SerializeField] private HealthConfigSO _healthConfig;
        public HealthSystem HealthSystem { get; private set; }
        public GameObject Behaviour => gameObject;

        public void OnHealthChanged(float prevAmount) {
            GameManager._instance.IncrementScore(HealthSystem.IsDead ? _deathScoreAmount : _damageScoreAmount);
        }
        
        public void OnDeath() {
            Destroy(gameObject);
        }
        
        private void Start() {
            HealthSystem = new HealthSystem(this, _healthConfig.Config);
        }
    }
}