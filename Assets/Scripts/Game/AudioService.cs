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

        private Dictionary<AudioSourceType, AudioSource> audioSources;

        private enum AudioSourceType {
            BGM,
            SFX
        }
        
        private void Start() {
            InitializeAudioSources();
            if (_playBGM) StartBGM();
            MessageBroker.Default.Receive<PlaySFXEvent>().Subscribe(x => PlaySFX(x.Clip)).AddTo(this);
        }
        
        private void InitializeAudioSources() {
            var bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(AudioSourceType.BGM.ToString()).First();
            var sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(AudioSourceType.SFX.ToString()).First();

            audioSources = new Dictionary<AudioSourceType, AudioSource> {
                {AudioSourceType.BGM, bgmSource},
                {AudioSourceType.SFX, sfxSource}
            };
        }

        public void PlaySFX(AudioClip clip) {
            var audioSource = audioSources[AudioSourceType.SFX];
            audioSource.PlayOneShot(clip);
        }
        
        private void StartBGM() {
            var audioSource = audioSources[AudioSourceType.BGM];
            Debug.Assert(audioSource.outputAudioMixerGroup != null);
            audioSource.clip = _bgm;
            audioSource.loop = true;
            audioSource.Play();
        }

    }
}