using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] float _speed;

    Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateTowards(_player.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody2D.AddForce((_player.transform.position - transform.position).normalized * _speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void RotateTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var offset = 0f;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }
}
