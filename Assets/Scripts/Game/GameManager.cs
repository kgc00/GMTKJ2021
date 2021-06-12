using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField] float _startingHealth = 100f;
    [SerializeField] float _damagePerHit = 10f;
    [SerializeField] TextMeshPro _scoreText;
    
    float _playerHealth;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerHealth = _startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.fontSize = Mathf.MoveTowards(_scoreText.fontSize, 5f, 0.075f);
        _scoreText.color = Color.Lerp(Color.red, Color.green, _playerHealth / _startingHealth);
        _scoreText.text = _playerHealth.ToString();
    }

    public void DamagePlayer()
    {
        _scoreText.fontSize = 15f;
        _playerHealth -= _damagePerHit;
        
        if(_playerHealth <= 0)
        {
            ResetLevel();
        }
    }

    void ResetLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
