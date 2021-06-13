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
        [SerializeField] private AudioClip _throwSFX;
        [SerializeField] private AudioClip _reelSFX;
        [SerializeField] private AudioClip _catchSFX;
        [SerializeField] private AudioClip _deflectSFX;
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
            MessageBroker.Default.Publish(new PlaySFXEvent(_reelSFX));
        }

        public void Catch(Anchor anchor, Collision2D collision2D) {
            if (_catching) {
                Destroy(anchor.gameObject);
                _inputReader.EnableDeploying();
                MessageBroker.Default.Publish(new PlaySFXEvent(_catchSFX));
            }
            else {
                anchor.Deflect(collision2D);
                MessageBroker.Default.Publish(new PlaySFXEvent(_deflectSFX));
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
            MessageBroker.Default.Publish(new PlaySFXEvent(_throwSFX));
        }
    }
}