
using DG.Tweening;
using Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class HealthBar : MonoBehaviour {
        [SerializeField] private Image _redFill; // parent
        [SerializeField] private Image _orangeFill; // child
        [SerializeField] private CanvasGroup _canvasGroup;
        private Sequence _sequence;

        private void Awake() {
            MessageBroker.Default.Receive<PlayerHealthChanged>().Subscribe(UpdateView).AddTo(this);
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void UpdateView(PlayerHealthChanged message) {
            var fillAmount = message.Adjustment.EndingHealth / message.Adjustment.MaxHealth;

            _sequence = DOTween.Sequence()
                .Append(DOTween.To(x => _canvasGroup.alpha = x, 
                    _canvasGroup.alpha, 
                    1, 
                    .1f))
                .Append(DOTween.To(x => _redFill.fillAmount = x,
                    _redFill.fillAmount,
                    fillAmount,
                    .12f))
                .Join(DOTween.To(x => _orangeFill.fillAmount = x,
                    _orangeFill.fillAmount,
                    fillAmount,
                    .22f))
                .Append(DOTween.To(x => _canvasGroup.alpha = x, 1, 0, 1f))
                .Play();
        }

        private void OnDestroy() => _sequence?.Kill();
    }
}