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

        [Header("SFX")]
        [SerializeField] private AudioClip _throwSFX;
        [SerializeField] private AudioClip _reelSFX;
        [SerializeField] private AudioClip _catchSFX;
        [SerializeField] private AudioClip _deflectSFX;

        [Header("VFX")]
        [SerializeField] private GameObject _deflectVFX;
        [SerializeField] private GameObject _catchVFX;
        [SerializeField] private GameObject _throwVFX;

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
            var pos = collision2D.gameObject.transform.position; // anchor user's position
            var offset = (collision2D.otherCollider.ClosestPoint(anchor.transform.position) - (Vector2)pos) / 2;

            if (_catching) {
                Destroy(anchor.gameObject);
                _inputReader.EnableDeploying();
                MessageBroker.Default.Publish(new PlaySFXEvent(_catchSFX));
                MessageBroker.Default.Publish(new VFXEvent(_catchVFX, transform));
            } else {
                anchor.Deflect(collision2D);
                MessageBroker.Default.Publish(new PlaySFXEvent(_deflectSFX));
                MessageBroker.Default.Publish(new VFXEvent(_deflectVFX, (Vector2)pos + (offset)));
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
            _distanceJoint.connectedBody = _anchorObj.GetComponent<Rigidbody2D>();
            _distanceJoint.distance = _distanceTheshold;
            _catching = false;
            _inputReader.EnableReeling();
            MessageBroker.Default.Publish(new PlaySFXEvent(_throwSFX));
            MessageBroker.Default.Publish(new VFXEvent(_throwVFX, spawnPos, spawnRot));
        }

        [SerializeField] DistanceJoint2D _distanceJoint;
        [SerializeField, Range(1, 20)] float _distanceTheshold;
        // public void FixedUpdate() {
        //     if (_anchorObj == null) return;

        //     var distance = Vector3.Distance(_anchorObj.transform.position, transform.position);
        //     if (distance <= _distanceThreshold) return;

        //     var heading = _anchorObj.transform.position - transform.position;
        //     heading.z = 0;
        //     gameObject.transform.position = heading.normalized * _distanceThreshold;
        // }
    }
}