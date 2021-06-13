using DG.Tweening;
using Mechanics.Health;
using UnityEngine;

namespace Mechanics {
    public partial class KnockbackDebuff : MonoBehaviour {
        private float _maxForce = 0.5f;
        private int Damage;
        public Vector2 Direction { get; private set; }
        public float Duration { get; private set; }
        public float Force { get; private set; }

        public void Initialize(KnockbackData data) {
            if (transform.root.gameObject.GetComponentsInChildren<KnockbackDebuff>().Length > 2) {
                Destroy(this);
            }

            Direction = data.Direction;
            Duration = data.Duration;
            Force = data.Force;
            Damage = data.Damage;
            if (TryGetComponent(out EnemyAI _ai)) {
                _ai.enabled = false;
            }

            if (TryGetComponent(out Rigidbody2D _rigidbody)) {
                _rigidbody.AddForce(Direction * Mathf.Clamp(Force, 1, _maxForce),
                    ForceMode2D.Impulse);
            }

            DOTween.Sequence().AppendInterval(Duration).AppendCallback(() => { Destroy(this); });
        }


        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Enemy")) {
                
                if (other.gameObject.TryGetComponent(out IDamagable damagable)) {
                    if (damagable as Object == null) return;

                    damagable.HealthSystem.Damage(Damage, this);
                }
                
                other.gameObject.AddComponent<KnockbackDebuff>()
                    .Initialize(new KnockbackData(other.contacts[0].normal, Force /2,Duration*0.7f, Damage));
            }
        }

        private void OnDestroy() {
            if (TryGetComponent(out EnemyAI _ai)) {
                _ai.enabled = true;
            }
        }
    }
}