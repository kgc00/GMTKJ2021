using Input;
using Messages;
using UniRx;
using UnityEngine;

namespace Units {
    public class AnchorUser : MonoBehaviour {
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private InputReader _inputReader;
        private GameObject _anchor;

        private void OnEnable() {
            MessageBroker.Default.Receive<DeployAction>().Subscribe(_ => HandleDeploy());
            MessageBroker.Default.Receive<ReelAction>().Subscribe(_ => HandleReel());
            MessageBroker.Default.Receive<ReelCanceledAction>().Subscribe(_ => HandleReelCanceled());
            
        }

        private void HandleReelCanceled() { }

        private void HandleReel() {
            Destroy(_anchor);
            _inputReader.EnableDeploying();
        }
        
        private void HandleDeploy() {
            _anchor = Instantiate(_anchorPrefab, transform.position, Quaternion.identity);
            _inputReader.EnableReeling();
        }
    }
}