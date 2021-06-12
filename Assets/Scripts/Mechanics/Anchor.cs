using Extensions;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Anchor : MonoBehaviour {
        [field: SerializeField] public Vector3 _throwTarget { get; private set; }
        [field: SerializeField] public Vector3 _deflectTarget { get; private set; }
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _speed;
        private AnchorState _state = AnchorState.Idle;
        private Transform _reelTransform;
        [SerializeField, Range(0, 10)]private float _deflectForce = 7;

        enum AnchorState {
            Idle,
            Thrown,
            Landed,
            Reeling,
            Caught,
            Deflected,
        }


        private void Update() {
            if (_state == AnchorState.Thrown) {
                if (Vector3.Distance(transform.position, _throwTarget) > 0.1) return;
                _state = AnchorState.Landed;
            }

            if (_state == AnchorState.Reeling) {
                if (Vector3.Distance(transform.position, _reelTransform.position) > 0.1) return;
                _state = AnchorState.Caught;
            }

            if (_state == AnchorState.Caught) { }

            if (_state == AnchorState.Deflected) {
                if (Vector3.Distance(transform.position, _deflectTarget) > 0.1) return;
                _state = AnchorState.Landed;
            }
        }

        private void FixedUpdate() {
            if (_state == AnchorState.Thrown) {
                var destination = _throwTarget - transform.position;
                _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed);
            }

            if (_state == AnchorState.Reeling) {
                var destination = _reelTransform.position - transform.position;
                _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed);
            }
            if (_state == AnchorState.Deflected) {
                var destination = deflectTarget - transform.position;
                _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed);
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
        Vector3 deflectTarget = Vector3.zero;
        public void Deflect(Collision2D collision2D) {
            _state = AnchorState.Deflected;
            deflectTarget = collision2D.gameObject.transform.position + (collision2D.contacts[0].normal * _deflectForce).ToVector3();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (_state == AnchorState.Reeling) {
                if (!collision.gameObject.TryGetComponent(out AnchorUser anchorUser)) return;

                anchorUser.Catch(this, collision);
            }
        }

        // private void OnDrawGizmos() {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawSphere(deflectTarget, 1);
        // }
    }
}