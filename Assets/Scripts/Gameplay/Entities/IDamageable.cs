
namespace Gameplay.Entities
{
    public interface IDamageable
    {
        bool IsBoss { get; }
        void ApplyDamage(float damage, bool critical = false, bool mute = false, bool fatal = false);
        void ApplySlow(float duration);
    }
}
