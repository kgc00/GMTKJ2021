using Input;
using Messages;
using UniRx;
using UnityEditor.VersionControl;
using UnityEngine;
using Utils;

namespace Mechanics {
    [RequireComponent(typeof(Movement))]
    public class AnchorUser : MonoBehaviour {
        [SerializeField] private Movement _movement;
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private InputReader _inputReader;
        private Anchor _anchorObj;
        private Vector3 _mousePos;
        private bool _catching;

        private void OnEnable() {
            MessageBroker.Default.Receive<DeployAction>().Subscribe(_ => HandleDeploy()).AddTo(this);
            MessageBroker.Default.Receive<ReelAction>().Subscribe(_ => HandleReel()).AddTo(this);
            MessageBroker.Default.Receive<ReelCanceledAction>().Subscribe(_ => HandleReelCanceled()).AddTo(this);
            MessageBroker.Default.Receive<PointAction>().Subscribe(input => HandlePoint(input.PointPos)).AddTo(this);
        }

        private void HandlePoint(Vector2 inputPointPos) {
            _mousePos = inputPointPos;
        }

        private void HandleReelCanceled() {
            _catching = false;
        }

        private void HandleReel() {
            _catching = true;
            _anchorObj.Reel(transform);
        }

        public void Catch(Anchor anchor, Collision2D collision2D) {
            if (_catching) {
                Destroy(anchor.gameObject);
                _inputReader.EnableDeploying();
            }
            else {
                anchor.Deflect(collision2D);
            }
        }

        private void HandleDeploy() {
            var throwPos = GameUtils.MousePosToWorldPosition(_mousePos);
            var spawnPos = transform.position + transform.right;
            var lookTarget = throwPos - spawnPos;
            float angle = Mathf.Atan2(lookTarget.y, lookTarget.x) * Mathf.Rad2Deg;
            var spawnRot = Quaternion.AngleAxis(angle, Vector3.forward);
            _anchorObj = Instantiate(_anchorPrefab, spawnPos, spawnRot).GetComponent<Anchor>();
            _anchorObj.Throw(throwPos);
            _catching = false;
            _inputReader.EnableReeling();
        }
    }
}