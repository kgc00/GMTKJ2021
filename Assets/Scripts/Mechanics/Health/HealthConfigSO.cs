using UnityEngine;

namespace Mechanics.Health {
    [CreateAssetMenu(fileName = "HealthConfigSO", menuName = "Game/HealthConfigSO", order = 0)]
    public class HealthConfigSO : ScriptableObject {
        [SerializeField] public HealthConfig Config;
    }
}