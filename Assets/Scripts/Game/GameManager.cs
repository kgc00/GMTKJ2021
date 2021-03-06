using System.Globalization;
using DG.Tweening;
using Input;
using Messages;
using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private InputReader _inputReader;
    public static GameManager _instance;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _instructionText;
    private float _score;
    [SerializeField, Range(0,10)] private float _incrementFrequency;
    [SerializeField, Range(0,10)] private int _incrementAmount;
    [SerializeField] private Color _scoreTextBaseColor;
    [SerializeField] private Color _scoreTextKillColor;
    [SerializeField] private AudioClip _incrementSound;
    private Sequence _recurringIncrementSequence;
    private Sequence _incrementVisualsSequence;
    private Sequence _decreaseInstructionsOpacitySequence;

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
        _incrementVisualsSequence?.Kill();
    }

    // Start is called before the first frame update
    void Start() {
        _inputReader.EnableDeploying();
        _score = 0;
        _recurringIncrementSequence = DOTween.Sequence()
            .AppendInterval(_incrementFrequency)
            .AppendCallback(() => IncrementScore(_incrementAmount, false))
            .SetLoops(-1).Play();

        _incrementVisualsSequence = DOTween.Sequence()
            .AppendCallback(() => {MessageBroker.Default.Publish(new PlaySFXEvent(_incrementSound));})
            .Append(_scoreText.materialForRendering.DOColor(_scoreTextKillColor, "_FaceColor", .1f))
            .Append(_scoreText.transform.DOPunchScale(Vector3.one, .2f))
            .Join(_scoreText.materialForRendering.DOColor(_scoreTextBaseColor, "_FaceColor", .2f))
            .SetAutoKill(false)
            .Pause();
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
