using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity {
    public class DropItem : MonoBehaviour {
        [HideInInspector] public static HPManager hpManager => GameManager.Instance.HPManager;
        [HideInInspector] public static PlayerManager playerManager => GameManager.Instance.PlayerManager;
        private static EffectManager effectManager;
        public SpriteRenderer sprite;
        public Sprite[] itemSprites;
        public int type = 0;

        private void Start() {
            if (effectManager == null)
                effectManager = GameObject.Find("Effects").GetComponent<EffectManager>();
        }
        
        public void SetType(int n) {
            type = n;
            sprite.sprite = itemSprites[n];
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.transform.tag == "Player") {
                switch ((ItemType)type) {
                    case ItemType.Wing :
                        playerManager.GetWing(1);
                        break;
                    case ItemType.Barrier:
                        hpManager.GetBarrier(1);
                        break;
                    case ItemType.HP:
                        hpManager.ChangeHP(10);
                        break;
                    case ItemType.Damage:
                        playerManager.DamageAllEnemy(playerManager.damage * playerManager.damageCoefficient + playerManager.fixDamage);
                        break;
                }
                if(playerManager.repair)      hpManager.ChangeHP(10);
                if(playerManager.jera)        playerManager.damage += 0.2f;
                if(playerManager.dagaz)       hpManager.GetBarrier(1);
                if(playerManager.reinforce)   playerManager.criticalProb += 0.1f;
                effectManager.PlayItemSound();
                gameObject.SetActive(false);
            }
        }
    }
}