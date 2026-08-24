using UnityEngine;

namespace Gameplay.Entities
{
    public sealed class EnemyHealth
    {
        public float Max { get; private set; } = 2f;
        public float Current { get; private set; } = 2f;

        public void SetMaximum(float maximum)
        {
            Max = maximum;
            Current = Max;
        }

        public void IncreaseMaximum(float amount)
        {
            Max += amount;
            Current = Max;
        }

        public float TakeDamage(float damage)
        {
            var appliedDamage = Mathf.Max(damage, 0f);
            Current -= appliedDamage;
            return appliedDamage;
        }

        public float Heal(float amount)
        {
            if (Max - Current <= 0.0001f)
            {
                return 0f;
            }

            var healedAmount = Mathf.Min(amount, Max - Current);
            Current += healedAmount;
            return healedAmount;
        }
    }
}
