using Extensions;
using Mechanics.Health;
using Messages;
using UniRx;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Anchor : MonoBehaviour {
        public Vector3 _throwTarget { get; private set; }
        public Vector3 _deflectTarget { get; private set; }


        private AnchorState _state = AnchorState.Idle;

        private Transform _reelTransform;

        [Header("General")]
        [SerializeField, Range(1, 10)] private int _damage;
        [SerializeField, Range(0.5f, 5f)] private float _knockbackDuration;
        [SerializeField] private GameObject _hitVFX;
        [SerializeField] private GameObject _knockbackVFX;
        [SerializeField] private Rigidbody2D _rigidbody;


        [Header("Physics")]
        [SerializeField, Range(0, 10)]
        private float _accelFactor = 1;

        [SerializeField, Range(1, 10)] private float _maxAccelFactor = 2;
        [SerializeField, Range(10, 50f)] private float _maxSpeed;
        [SerializeField, Range(0.1f, .99f)] private float _destinationDampFactor;
        [SerializeField, Range(5f, 50f)] private float _speed;
        [SerializeField, Range(0.01f, 2f)] private double _acceptanceThreshold = 0.1;
        [SerializeField, Range(0, 2f)] private float _deflectSpeed;
        [SerializeField] private bool deflectWithRelativeVelocity;

        [SerializeField, Range(0, 10), Header("if deflectWithRelativeVelocity == true:")]
        private float _deflectForce = 0.8f;

        [SerializeField, Range(0, 10), Header("if deflectWithRelativeVelocity == false:")]
        private float _relativeDeflectForce = 7;

        private float _baseAccelFactor;
        private Vector2 _previousVelocity;
        private Vector2 _lastPos;

        enum AnchorState {
            Idle,
            Thrown,
            Landed,
            Reeling,
            Caught,
            Deflected,
        }

        private void Start() {
            _baseAccelFactor = _accelFactor;
        }

        private void Update() {
            if (_state == AnchorState.Thrown) {
                if (Vector3.Distance(transform.position, _throwTarget) > _acceptanceThreshold) return;
                _state = AnchorState.Landed;
            }

            if (_state == AnchorState.Reeling) {
                if (_reelTransform == null) return;

                if (Vector3.Distance(transform.position, _reelTransform.position) > _acceptanceThreshold / 2f) return;
                _state = AnchorState.Caught;
            }

            if (_state == AnchorState.Caught) { }

            if (_state == AnchorState.Deflected) {
                if (Vector3.Distance(transform.position, _deflectTarget) > _acceptanceThreshold) return;
                _accelFactor = _baseAccelFactor;
                _state = AnchorState.Landed;
            }
        }

        private void FixedUpdate() {
            if (_state == AnchorState.Thrown) {
                var destination = (_throwTarget - transform.position).normalized;
                _rigidbody.velocity += (Vector2)destination * _speed;
                _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
            }

            if (_state == AnchorState.Reeling) {
                if (_reelTransform == null) return;

                _accelFactor = Mathf.Min(_accelFactor + Time.fixedDeltaTime, _maxAccelFactor);
                var destination = (_reelTransform.position - transform.position).normalized;
                _rigidbody.velocity += (Vector2)destination * _speed * _accelFactor * Time.fixedDeltaTime;
                _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
            }

            if (_state == AnchorState.Deflected) {
                var destination = (_deflectTarget - transform.position).normalized;
                _rigidbody.velocity += (Vector2)destination * _speed * _deflectSpeed;
                _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
            }

            if (_state == AnchorState.Landed) {
                _rigidbody.velocity *= _destinationDampFactor;
            }
        }

        public void Throw(Vector3 mousePos) {
            _throwTarget = mousePos;
            _state = AnchorState.Thrown;
        }

        public void Reel(Transform anchorUserTransform) {
            _reelTransform = anchorUserTransform;
            _state = AnchorState.Reeling;
        }

        public void Deflect(Collision2D collision2D) {
            if (_state == AnchorState.Thrown) return;
            _state = AnchorState.Deflected;
            var baseDeflectPos = (collision2D.contacts[0].normal * _deflectForce).ToVector3();
            var relativeForceDeflectPos = collision2D.relativeVelocity.magnitude *
                                          (collision2D.contacts[0].normal * _relativeDeflectForce).ToVector3();
            var deflectPos = deflectWithRelativeVelocity ? relativeForceDeflectPos : baseDeflectPos;
            var otherPos = collision2D.gameObject.transform.position;
            _deflectTarget = (Vector2)(otherPos + deflectPos);

        }

        private void OnCollisionEnter2D(Collision2D collision) {
            HandleEnemyCollision(collision);

            HandlePlayerCollision(collision);
        }

        private void OnCollisionStay2D(Collision2D collision) {
            HandlePlayerCollision(collision);
        }

        private void HandlePlayerCollision(Collision2D collision) {
            if (!collision.gameObject.TryGetComponent(out AnchorUser anchorUser)) return;

            if (_state == AnchorState.Reeling) {
                _accelFactor = _baseAccelFactor;
                anchorUser.Catch(this, collision);
            }
        }

        private void HandleEnemyCollision(Collision2D collision) {
            if (!collision.gameObject.CompareTag("Enemy")) return;

            if (collision.gameObject.TryGetComponent(out IDamagable damagable)) {
                if (damagable as Object == null) return;

                damagable.HealthSystem.Damage(_damage, this);
                var lookTarget = collision.transform.position - transform.position;
                float angle = Mathf.Atan2(lookTarget.y, lookTarget.x) * Mathf.Rad2Deg;
                var spawnRot = Quaternion.AngleAxis(angle, Vector3.forward);
                MessageBroker.Default.Publish(new VFXEvent(_hitVFX, collision.otherCollider.ClosestPoint(transform.position), spawnRot));
            }

            collision.gameObject.AddComponent<KnockbackDebuff>().Initialize(new KnockbackData(collision.contacts[0].normal,
                collision.relativeVelocity.sqrMagnitude, _knockbackDuration, _damage, _knockbackVFX));
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_deflectTarget, (float)_acceptanceThreshold * 2f);
        }
    }
}