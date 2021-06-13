using Input;
using Messages;
using System;
using UniRx;
using UnityEngine;
using Utils;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Rigidbody2D _rigidbody;
        public Vector2 Motion { get; private set; }
        [SerializeField, Range(1,1000)] private float _speed = 15f;
        [SerializeField, Range(1,500)] private float _maxSpeed = 15f;
        [SerializeField, Range(0f, 5f)] private float _dashDiminishRate = 0.05f;
        [SerializeField, Range(5f, 1000f)] private float _dashSpeed = 15f;
        
        private Vector2 _pointerPos;
        private float _aDASHionableSpeed;
        private MovementState _state;
        private TrailRenderer _trailRenderer;

        public enum MovementState
        {
            Dashing,
            NotDashing
        }

        private void OnEnable() {
            MessageBroker.Default.Receive<MoveActionPerformed>().Subscribe(input => HandleMove(input.Motion)).AddTo(this);
            MessageBroker.Default.Receive<MoveActionCanceled>().Subscribe(input => HandleMove(Vector2.zero)).AddTo(this);
            MessageBroker.Default.Receive<PointAction>().Subscribe(input => HandlePoint(input.PointPos)).AddTo(this);
            MessageBroker.Default.Receive<DashAction>().Subscribe(input => HandleDash()).AddTo(this);

            _state = MovementState.NotDashing;
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        private void OnGUI()
        {
            GUILayout.Box(_aDASHionableSpeed.ToString());
            GUILayout.Box(_state.ToString());
        }

        private void HandlePoint(Vector2 inputPointPos) {
            _pointerPos = inputPointPos;
        }

        private void HandleMove(Vector2 inputMotion) {
            Motion = inputMotion;
        }

        private void HandleDash() {
            SwitchState(MovementState.Dashing);
        }
        

        private void Update() {
            UpdateRotation();
            UpdateDashSpeed();
        }

        private void UpdateDashSpeed()
        {
            _aDASHionableSpeed = Mathf.MoveTowards(_aDASHionableSpeed, 0, _dashDiminishRate);
            if (_aDASHionableSpeed < 0.5f) {
                SwitchState(MovementState.NotDashing);
            }
        }

        private void UpdateRotation() {
            var mousePos = GameUtils.MousePosToWorldPosition(_pointerPos);
            var lookTarget = mousePos - transform.position;
            float angle = Mathf.Atan2(lookTarget.y,lookTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void FixedUpdate() {
            _rigidbody.velocity = Motion.normalized * Time.fixedDeltaTime * (_speed + _aDASHionableSpeed);
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed + _aDASHionableSpeed);
        }

        void SwitchState(MovementState newState) {
            switch (_state)
            {
                case MovementState.Dashing:
                    if (newState == MovementState.Dashing)
                        return;
                    break;
                case MovementState.NotDashing:
                    break;
                default:
                    break;
            }
            _state = newState;
            switch (_state)
            {
                case MovementState.Dashing:
                    _aDASHionableSpeed = _dashSpeed;
                    _trailRenderer.emitting = true;
                    break;
                case MovementState.NotDashing:
                    _trailRenderer.emitting = false;
                    break;
                default:
                    break;
            }
        }
    }
}