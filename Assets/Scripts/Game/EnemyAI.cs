using System.Collections;
using System.Collections.Generic;
using Mechanics.Health;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    [SerializeField] GameObject _player;
    [SerializeField] float _speed;
    [SerializeField, Range(0, 10)] private float _speedCap;

    Rigidbody2D _rigidbody2D;
    private AIState _state;
    [SerializeField, Range(0, 10)] private float _damage;

    enum AIState {
        Chasing,
        Knockback,
    }

    // Start is called before the first frame update
    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _state = AIState.Chasing;
    }

    private void Update() {
        if (_player == null) return;
        RotateTowards(_player.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_player == null) return;
        var normalizedTarget = (_player.transform.position - transform.position).normalized;
        _rigidbody2D.AddForce(normalizedTarget * _speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, _speedCap);
    }

    private void RotateTowards(Vector2 target) {
        Vector2 direction = (target - (Vector2) transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var offset = 0f;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        HandlePlayerCollision(collision);
    }

    private void HandlePlayerCollision(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        if (collision.gameObject.TryGetComponent(out IDamagable damagable)) {
            if (damagable as Object == null) return;

            damagable.HealthSystem.Damage(_damage, this);
        }

        Destroy(gameObject);
    }
}