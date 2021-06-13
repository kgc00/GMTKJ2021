using System.Collections.Generic;
using System.Linq;
using Mechanics.Health;
using Messages;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace Game {
    public class AudioService : MonoBehaviour {
        
        [Header("Settings")] [SerializeField] private AudioMixer _mixer;

        [Header("BGM")] [SerializeField] private bool _playBGM;
        [SerializeField] private AudioClip _bgm;

        // [Header("Sfx")] [SerializeField] private AudioClip _click;
        // [SerializeField] private AudioClip _playerAttack;
        // [SerializeField] private AudioClip _playerDefend;
        // [SerializeField] private AudioClip _playerHit;
        // [SerializeField] private AudioClip _playerPreHit;
        // [SerializeField] private AudioClip _playerMiss;
        // [SerializeField] private AudioClip _enemyAttackConnected;

        private Dictionary<AudioSourceType, AudioSource> _audioSources;

        private enum AudioSourceType {
            BGM,
            SFX
        }
        
        private void Start() {
            InitializeAudioSources();
            if (_playBGM) StartBGM();
            // MessageBroker.Default.Receive<CombatControlClick>().Subscribe(_ => ClickSfx()).AddTo(this);
            // MessageBroker.Default.Receive<PlayerAttackConnected>().Subscribe(_ => PlayerAttackConnectedSfx())
            //     .AddTo(this);
            // MessageBroker.Default.Receive<PlayerAttackMiss>().Subscribe(_ => PlayerAttackMissSfx()).AddTo(this);
            // MessageBroker.Default.Receive<PlayerDeflectedAttack>().Subscribe(_ => PlayerDeflectSfx()).AddTo(this);
            // MessageBroker.Default.Receive<EnemyAttackConnected>().Subscribe(_ => EnemyAttackConnectedSfx()).AddTo(this);
            // MessageBroker.Default.Receive<PlayerHealthChanged>()
            //     .Where(x => x.Adjustment.Result == HealthAdjustment.AdjustmentResult.Damaged)
            //     .Subscribe(_ => PlayerDamageSfx()).AddTo(this);
            MessageBroker.Default.Receive<PlaySFXEvent>().Subscribe(x => PlaySFX(x.Clip)).AddTo(this);
        }
        
        private void InitializeAudioSources() {
            var bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(AudioSourceType.BGM.ToString()).First();
            var sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(AudioSourceType.SFX.ToString()).First();

            _audioSources = new Dictionary<AudioSourceType, AudioSource> {
                {AudioSourceType.BGM, bgmSource},
                {AudioSourceType.SFX, sfxSource}
            };
        }

        public void PlaySFX(AudioClip clip) {
            var audioSource = _audioSources[AudioSourceType.SFX];
            audioSource.PlayOneShot(clip);
        }

        
        private void StartBGM() {
            var audioSource = _audioSources[AudioSourceType.BGM];
            Debug.Assert(audioSource.outputAudioMixerGroup != null);
            audioSource.clip = _bgm;
            audioSource.loop = true;
            audioSource.Play();
        }

    }
}