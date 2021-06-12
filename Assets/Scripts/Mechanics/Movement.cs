using Input;
using Messages;
using UniRx;
using UnityEngine;

namespace Units {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Rigidbody2D _rigidbody;
        private Vector2 _motion;
        [SerializeField, Range(1,20)] private float _speed = 5f;

        private void OnEnable() {
            MessageBroker.Default.Receive<MoveActionPerformed>().Subscribe(input => HandleMove(input.Motion));
            MessageBroker.Default.Receive<MoveActionCanceled>().Subscribe(input => HandleMove(Vector2.zero));
        }

        private void HandleMove(Vector2 inputMotion) {
            _motion = inputMotion;
        }

        private void HandleFire() { }

        private void FixedUpdate() {
            // _rigidbody.velocity += _motion * Time.fixedDeltaTime * _speed;
            _rigidbody.MovePosition(_rigidbody.position + _motion * Time.fixedDeltaTime * _speed);
        }
    }
}