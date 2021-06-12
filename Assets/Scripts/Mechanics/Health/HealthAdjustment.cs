namespace Mechanics.Health {
    public class HealthAdjustment {
        public  readonly  float StartingHealth;
        public  readonly  float EndingHealth;
        public  readonly  float MaxHealth;
        public HealthAdjustment(float startingHealth, float endingHealth, float maxHealth) {
            StartingHealth = startingHealth;
            EndingHealth = endingHealth;
            MaxHealth = maxHealth;
        }
    }
}