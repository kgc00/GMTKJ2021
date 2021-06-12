using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Anchor : MonoBehaviour {
        [field: SerializeField] public Vector3 _targetPosition;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _speed;

        private void FixedUpdate() {
            var destination = _targetPosition - transform.position;
            _rigidbody.MovePosition(transform.position + destination * Time.fixedDeltaTime * _speed);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_targetPosition, 1f);
        }
    }
}