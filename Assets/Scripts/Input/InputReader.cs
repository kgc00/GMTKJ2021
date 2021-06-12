using Messages;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/InputReader", order = 0)]
    public class InputReader : ScriptableObject, Controls.IGameplayActions {
        private Controls controls;

        private void OnEnable() {
            controls ??= new Controls();
            controls.Gameplay.SetCallbacks(this);
            controls.Gameplay.Enable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            var readValue = context.ReadValue<Vector2>();
            if (context.performed) MessageBroker.Default.Publish(new MoveActionPerformed(readValue));
            if (context.canceled) MessageBroker.Default.Publish(new MoveActionCanceled());
        }

        public void OnDeploy(InputAction.CallbackContext context) { 
            if(context.performed) MessageBroker.Default.Publish(new DeployAction());
        }

        public void OnReel(InputAction.CallbackContext context) {
            if(context.canceled) MessageBroker.Default.Publish(new ReelCanceledAction());
            if(context.performed) MessageBroker.Default.Publish(new ReelAction());
        }

        public void OnDash(InputAction.CallbackContext context) {
            if (context.performed) MessageBroker.Default.Publish(new DashAction());
        }

        public void EnableReeling() {
            controls.Gameplay.Deploy.Disable();
            controls.Gameplay.Reel.Enable();
        }

        public void EnableDeploying() {
            controls.Gameplay.Reel.Disable();
            controls.Gameplay.Deploy.Enable();
        }
    }
}
