using UnityEngine;
using Gameplay.Effects;

namespace Gameplay.Managers
{
    public class EffectManager : MonoBehaviour, IDependencyInjectable
    {
        private PoolManager pool;

        public void InjectDependency(GameDependencies dependencies)
        {
            pool = dependencies.PoolManager;
        }

        public void SetDamageEffect(Vector3 pos, float dmg, bool isCritical = false, bool isFatal = false, bool isHeal = false)
        {
            var myVector = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            var effect = pool.Spawn<DamageEffect>();
            effect.transform.position = pos + myVector;
            dmg = Mathf.Round(dmg * 100) * 0.01f;
            if (isHeal)
            {
                effect.SetEffectText(dmg.ToString(), Color.green);
            }
            else if (isFatal)
            {
                effect.SetEffectText("X.X", Color.red);
            }
            else if (isCritical)
            {
                effect.SetEffectText($"{dmg}!", Color.yellow);
            }
            else
            {
                effect.SetEffectText(dmg.ToString(), Color.white);
            }
        }
    }
}
