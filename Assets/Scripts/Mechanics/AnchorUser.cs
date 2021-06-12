using Extensions;
using Input;
using Messages;
using UniRx;
using UnityEngine;
using Utils;

namespace Mechanics {
    [RequireComponent(typeof(Movement))]
    public class AnchorUser : MonoBehaviour {
        [SerializeField] private Movement _movement;
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private InputReader _inputReader;
        private GameObject _anchorObj;
        private Vector3 _mousePos;

        private void OnEnable() {
            MessageBroker.Default.Receive<DeployAction>().Subscribe(_ => HandleDeploy());
            MessageBroker.Default.Receive<ReelAction>().Subscribe(_ => HandleReel());
            MessageBroker.Default.Receive<ReelCanceledAction>().Subscribe(_ => HandleReelCanceled());
            MessageBroker.Default.Receive<PointAction>().Subscribe(input => HandlePoint(input.PointPos));
        }

        private void HandlePoint(Vector2 inputPointPos) {
            _mousePos = GameUtils.MousePosToWorldPosition(inputPointPos);
        }

        private void HandleReelCanceled() { }

        private void HandleReel() {
            Destroy(_anchorObj);
            _inputReader.EnableDeploying();
        }
        
        private void HandleDeploy() {
            var spawnPos = transform.position + transform.right;
            _anchorObj = Instantiate(_anchorPrefab, spawnPos, Quaternion.identity);
            _anchorObj.GetComponent<Anchor>()._targetPosition = _mousePos;
            _inputReader.EnableReeling();
        }
    }
}