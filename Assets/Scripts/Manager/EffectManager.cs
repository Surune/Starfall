using UnityEngine;
using Starfall.Effect;
using Starfall.Constants;
using UnityEngine.Serialization;

namespace Starfall.Manager
{
    public class EffectManager : MonoBehaviour
    {
        PoolManager pool => GameManager.Instance.PoolManager;

        public void SetDamageEffect(Vector3 pos, float dmg, bool isCritical = false, bool isFatal = false, bool isHeal = false)
        {
            var myVector = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            var effect = pool.Get(PoolNumber.Effect).GetComponent<DamageEffect>();
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
