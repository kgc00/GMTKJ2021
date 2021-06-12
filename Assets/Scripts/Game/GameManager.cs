using System.Globalization;
using DG.Tweening;
using Input;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private InputReader _inputReader;
    public static GameManager _instance;
    [SerializeField] TextMeshProUGUI _scoreText;
    private float _score;
    [SerializeField, Range(0,10)] private float _incrementFrequency;
    [SerializeField, Range(0,10)] private int _incrementAmount;
    private Sequence _recurringIncrementSequence;
    private Sequence _incrementVisualsSequence;

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
        _recurringIncrementSequence?.Kill();
    }

    // Start is called before the first frame update
    void Start() {
        _inputReader.EnableDeploying();
        _score = 0;
        _scoreText.color = Color.green;
        _recurringIncrementSequence = DOTween.Sequence()
            .AppendInterval(_incrementFrequency)
            .AppendCallback(() => IncrementScore(_incrementAmount, false))
            .SetLoops(-1).Play();

        _incrementVisualsSequence = DOTween.Sequence()
            .Append(_scoreText.material.DOColor(Color.red, "_FaceColor", .05f))
            .Append(_scoreText.transform.DOPunchScale(Vector3.one, .2f))
            .Join(_scoreText.material.DOColor(Color.green, "_FaceColor", .1f))
            .SetAutoKill(false);
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.text = _score.ToString(CultureInfo.InvariantCulture);
    }

    public void IncrementScore(int addition, bool withSequence = true) {
        if (withSequence) _incrementVisualsSequence.Restart();
        _score += addition;
    }

    public void ResetLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
