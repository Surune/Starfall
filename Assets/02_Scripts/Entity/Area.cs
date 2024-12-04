using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Starfall.Manager;

namespace Starfall.Entity {
    public class Area : MonoBehaviour {
        #region Manager
        private static EffectManager effectManager => GameManager.Instance.EffectManager;
        private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
        #endregion

        public GameObject iconimage;
        public Image durationimage;
        public float duration;
        private float nowtime;
        [HideInInspector] public bool slow = false;
        [HideInInspector] public bool damage = false;
        private bool makecritical = false;
        private bool unrelenting = false;
        private bool fixdmg = false;
        private bool swarm = false;
        public List<GameObject> enemies;

        private void OnEnable() {
            SetProperty();
            effectManager.PlayAreaSound(start : true);
            enemies = new List<GameObject>();
            nowtime = 0f;
        }

        public void SetIcon(Sprite icon) {
            iconimage.GetComponent<Image>().sprite = icon;
        }

        public void SetProperty(bool isSlow = false, bool isDamage = false, bool isCrit = false, bool isUnrelenting = false, bool isFixed = false, bool isSwarm = false) {
            slow = isSlow;
            damage = isDamage;
            makecritical = isCrit;
            unrelenting = isUnrelenting;
            fixdmg = isFixed;
            swarm = isSwarm;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Enemy") { // If the object is tagged as an object
                enemies.Add(collision.gameObject); // Add the Object to the List
            }
            else if (collision.transform.tag == "Bullet") {
                if (makecritical && !collision.gameObject.GetComponent<Fireball>().isCritical)
                    playerManager.MakeCritical(collision.gameObject.GetComponent<Fireball>());
                else if (unrelenting && collision.gameObject.GetComponent<Fireball>().isCritical)
                    collision.gameObject.GetComponent<Fireball>().damage *= 2f;
                else if (fixdmg)
                    collision.gameObject.GetComponent<Fireball>().damage += 1f;
                else if (swarm)
                    collision.gameObject.GetComponent<Fireball>().psychosink = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.transform.tag == "Enemy") { // If the object is tagged as an object
                enemies.Remove(collision.gameObject); // Remove the Object from the List
            }
        }

        void Update() {
            if(GameStateManager.Instance.IsPlaying) {
                nowtime += Time.deltaTime;
                if (nowtime >= duration) {
                    if (damage) {
                        for (int i = enemies.Count - 1; i >= 0; i--) {
                            if(enemies[i] != null) {
                                enemies[i].GetComponent<Enemy>().GetDamage(4, mute: false);
                            }
                        }
                    }
                    if (slow) {
                        for (int i = enemies.Count - 1; i >= 0; i--) {
                            if(enemies[i] != null)  enemies[i].GetComponent<Enemy>().slowTime = 3f;
                        }
                    }
                    effectManager.PlayAreaSound(start : false);
                    gameObject.SetActive(false);
                }
                else 
                    durationimage.fillAmount = nowtime / duration;
            }
        }
    }
}