using Mechanics.Health;

namespace Messages {
    public class PlayerHealthChanged {
        public readonly HealthAdjustment Adjustment;
        public PlayerHealthChanged(HealthAdjustment adjustment) {
            Adjustment = adjustment;
        }
    }
}