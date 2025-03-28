using UnityEngine;
using Starfall.Effect;
using Starfall.Constants;
using UnityEngine.Serialization;

namespace Starfall.Manager
{
    public class EffectManager : MonoBehaviour
    {
        PoolManager pool => GameManager.Instance.PoolManager;

        public Exp Exp;

        public void SetDamageEffect(Vector3 pos, float dmg, bool isCritical = false, bool isFatal = false, bool isHeal = false)
        {
            var myVector = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            var effect = pool.Get(PoolNumber.Effect);
            effect.transform.position = pos + myVector;
            dmg = Mathf.Round(dmg * 100) * 0.01f;
            effect.transform.GetComponent<DamageEffect>().Delay = 0.25f;
            if (isHeal)
            {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "+" + dmg, Color.green);
            }
            else if (isFatal)
            {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "X_X", Color.red);
            }
            else if (isCritical)
            {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "-" + dmg + "!", Color.yellow);
            }
            else
            {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "-" + dmg, Color.white);
            }
        }

        void SetEffectText(DamageEffect effect, string text, Color color)
        {
            effect.AccumulatedTime = 0f;
            effect.ResourceText.text = text;
            effect.ResourceText.color = color;
        }
    }
}
