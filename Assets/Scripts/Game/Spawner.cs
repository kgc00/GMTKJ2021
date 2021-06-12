using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _spawningSurface;
    [SerializeField] private GameObject _toSpawn;
    [SerializeField] private float _spawnTime;

    private Bounds _spawningBounds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        bool clampX = true;
        while(true)
        {
            Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            _spawningBounds = _spawningSurface.GetComponent<SpriteRenderer>().bounds;

            Vector2 spawningBounds;
            if (clampX)
            {
                float clampedX = random.x > 0 ? _spawningBounds.center.x + _spawningBounds.extents.x : _spawningBounds.center.x - _spawningBounds.extents.x;
                spawningBounds = new Vector2(clampedX, _spawningBounds.center.y + random.y * _spawningBounds.extents.y);
            }
            else
            {
                float clampedY = random.y > 0 ? _spawningBounds.center.y + _spawningBounds.extents.y : _spawningBounds.center.y - _spawningBounds.extents.y;
                spawningBounds = new Vector2(_spawningBounds.center.x + random.x * _spawningBounds.extents.x, clampedY);
            }

            clampX = !clampX;

            yield return new WaitForSeconds(_spawnTime);
        }
    }
}
