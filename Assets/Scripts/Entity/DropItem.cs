using UnityEngine;
using UnityEngine.UI;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity
{
    public class DropItem : MonoBehaviour
    {
        static HPManager HpManager => GameManager.Instance.HPManager;
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
        static EffectManager EffectManager => GameManager.Instance.EffectManager;
        static SFXManager SfxManager => GameManager.Instance.SfxManager;
        public Image Image;
        public Sprite[] ItemSprites;
        public ItemType Type = 0;

        public void SetType(ItemType n)
        {
            Type = n;
            Image.sprite = ItemSprites[(int)n];
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Player"))
            {
                switch (Type)
                {
                    case ItemType.Wing:
                        PlayerManager.GetWing(1);
                        break;
                    case ItemType.Barrier:
                        HpManager.GetBarrier(1);
                        break;
                    case ItemType.HP:
                        HpManager.ChangeHP(10);
                        break;
                    case ItemType.Damage:
                        PlayerManager.DamageAllEnemy(PlayerManager.damage * PlayerManager.damageCoefficient + PlayerManager.fixDamage);
                        break;
                }
                if (PlayerManager.repair)
                {
                    HpManager.ChangeHP(10);
                }
                if (PlayerManager.jera)
                {
                    PlayerManager.damage += 0.2f;
                }
                if (PlayerManager.dagaz)
                {
                    HpManager.GetBarrier(1);
                }
                if (PlayerManager.reinforce)
                {
                    PlayerManager.criticalProb += 0.1f;
                }
                SfxManager.PlayItemSound();
                gameObject.SetActive(false);
            }
        }
    }
}
