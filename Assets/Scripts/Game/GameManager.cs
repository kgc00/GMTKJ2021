using System.Globalization;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [SerializeField] TextMeshPro _scoreText;
    private float _score;
    [SerializeField] private float _incrementFrequency;
    private Sequence _incrementSequence;

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

    private void OnDisable() {
        _incrementSequence?.Kill();
    }

    // Start is called before the first frame update
    void Start() {
        _score = 0;
        _scoreText.color = Color.green;
        _incrementSequence = DOTween.Sequence().AppendInterval(_incrementFrequency).AppendCallback(() => _score += 1).SetLoops(-1).Play();
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.fontSize = Mathf.MoveTowards(_scoreText.fontSize, 5f, 0.075f);
        _scoreText.text = _score.ToString(CultureInfo.InvariantCulture);
    }

    void ResetLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
