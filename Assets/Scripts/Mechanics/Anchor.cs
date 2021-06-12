using Extensions;
using Mechanics.Health;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Anchor : MonoBehaviour {
        [field: SerializeField] public Vector3 _throwTarget { get; private set; }
        [field: SerializeField] public Vector3 _deflectTarget { get; private set; }

        [SerializeField] private Rigidbody2D _rigidbody;
        
        private AnchorState _state = AnchorState.Idle;
        private Transform _reelTransform;

        [Header("Common")] [SerializeField, Range(0, 3)]
        private float _accelFactor = 1;

        [SerializeField, Range(5f, 50f)] private float _speed;
        [SerializeField, Range(1, 3)] private float _maxAccelFactor = 2;
        [SerializeField, Range(1, 10)] private float _damage;
        [SerializeField, Range(0.01f, 1f)] private double _acceptanceThreshold = 0.1;
        [SerializeField] private bool deflectWithRelativeVelocity;

        [SerializeField, Range(0, 10), Header("Used when deflectWithRelativeVelocity is true")]
        private float _deflectForce = 0.8f;

        [SerializeField, Range(0, 10), Header("Used when deflectWithRelativeVelocity is false")]
        private float _relativeDeflectForce = 7;

        private float _baseAccelFactor;
        Vector3 deflectTarget = Vector3.zero;

        private Vector3 _previousVelocity;

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

        private void OnGUI()
        {
            GUILayout.Box(_state.ToString());
            GUILayout.Box(_previousVelocity.magnitude.ToString());
        }

        private void Update() {
            if (_state == AnchorState.Thrown) {
                if (Vector3.Distance(transform.position, _throwTarget) > _acceptanceThreshold) return;
                _state = AnchorState.Landed;
            }

            if (_state == AnchorState.Reeling) {
                if (_reelTransform == null) return;

                if (Vector3.Distance(transform.position, _reelTransform.position) > _acceptanceThreshold/2f) return;
                _state = AnchorState.Caught;
            }

            if (_state == AnchorState.Caught) { }

            if (_state == AnchorState.Deflected) {
                if (Vector3.Distance(transform.position, _deflectTarget) > _acceptanceThreshold) return;
                _state = AnchorState.Landed;
            }
        }

        private void FixedUpdate() {
            if (_state == AnchorState.Thrown) {
                var destination = (_throwTarget - transform.position).normalized;
                Vector3 thrownVelocity = transform.position + destination * Time.fixedDeltaTime * _speed;
                _rigidbody.MovePosition(thrownVelocity);
                _previousVelocity = thrownVelocity;
            }

            if (_state == AnchorState.Reeling) {
                if (_reelTransform == null) return;
                
                _accelFactor = Mathf.Min(_accelFactor + Time.fixedDeltaTime, _maxAccelFactor);
                var destination = (_reelTransform.position - transform.position).normalized;
                _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed * _accelFactor);
            }

            if (_state == AnchorState.Deflected) {
                var destination = (deflectTarget - transform.position).normalized;
                _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed);
            }

            if (_state == AnchorState.Landed) {
                var destination = _previousVelocity;
                _rigidbody.MovePosition(transform.position + (_previousVelocity * 0.25f));
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
            _state = AnchorState.Deflected;
            var baseDeflectPos = (collision2D.contacts[0].normal * _deflectForce).ToVector3();
            var relativeForceDeflectPos = collision2D.relativeVelocity.magnitude *
                                          (collision2D.contacts[0].normal * _relativeDeflectForce).ToVector3();
            var deflectPos = deflectWithRelativeVelocity ? relativeForceDeflectPos : baseDeflectPos;
            deflectTarget = collision2D.gameObject.transform.position + deflectPos;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            HandleEnemyCollision(collision);

            HandlePlayerCollision(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            HandleEnemyCollision(collision);

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
            }
        }

        // private void OnDrawGizmos() {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawSphere(deflectTarget, 1);
        // }
    }
}