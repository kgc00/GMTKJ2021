using DG.Tweening;
using UnityEngine;

namespace Mechanics {
    public class KnockbackDebuff : MonoBehaviour {
        private float _maxForce = 0.5f;
        public Vector2 Direction { get; private set; }
        public float Duration { get; private set; }
        public float Force { get; private set; }

        public void Initialize(Vector2 knockbackDirection, float knockbackForce = 2f, float knockbackDuration=2f) {
            if (transform.root.gameObject.GetComponentsInChildren<KnockbackDebuff>().Length > 2) {
                Destroy(this);
            }

            Direction = knockbackDirection;
            Duration = knockbackDuration;
            Force = knockbackForce; 
            if (TryGetComponent(out EnemyAI _ai)) {
                _ai.enabled = false;
            }

            if (TryGetComponent(out Rigidbody2D _rigidbody)) {
                _rigidbody.AddForce(knockbackDirection * Mathf.Clamp(knockbackForce, 1, _maxForce), ForceMode2D.Impulse);
            }

            DOTween.Sequence().AppendInterval(Duration).AppendCallback(() => { Destroy(this); });
        }


        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Enemy")) {
                other.gameObject.AddComponent<KnockbackDebuff>()
                    .Initialize(other.contacts[0].normal, Force / 2, Duration * 0.75f);
            }
        }

        private void OnDestroy() {
            if (TryGetComponent(out EnemyAI _ai)) {
                _ai.enabled = true;
            }
        }
    }
}