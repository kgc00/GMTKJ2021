using Input;
using Messages;
using UniRx;
using UnityEngine;
using Utils;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Rigidbody2D _rigidbody;
        public Vector2 Motion { get; private set; }
        [SerializeField, Range(1,20)] private float _speed = 5f;
        private Vector2 _pointerPos;

        private void OnEnable() {
            MessageBroker.Default.Receive<MoveActionPerformed>().Subscribe(input => HandleMove(input.Motion)).AddTo(this);
            MessageBroker.Default.Receive<MoveActionCanceled>().Subscribe(input => HandleMove(Vector2.zero)).AddTo(this);
            MessageBroker.Default.Receive<PointAction>().Subscribe(input => HandlePoint(input.PointPos)).AddTo(this);
        }

        private void HandlePoint(Vector2 inputPointPos) {
            _pointerPos = inputPointPos;
        }

        private void HandleMove(Vector2 inputMotion) {
            Motion = inputMotion;
        }
        

        private void Update() {
            UpdateRotation();
        }

        private void UpdateRotation() {
            var mousePos = GameUtils.MousePosToWorldPosition(_pointerPos);
            var lookTarget = mousePos - transform.position;
            float angle = Mathf.Atan2(lookTarget.y,lookTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void FixedUpdate() {
            // _rigidbody.velocity += _motion * Time.fixedDeltaTime * _speed;
            _rigidbody.MovePosition(_rigidbody.position + Motion * Time.fixedDeltaTime * _speed);
        }
    }
}