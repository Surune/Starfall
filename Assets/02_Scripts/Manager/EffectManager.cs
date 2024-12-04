using UnityEngine;
using Starfall.Effect;
using Starfall.Constants;

namespace Starfall.Manager {
    public class EffectManager {
        private AudioSource musicplayer;

        # region Manager
        private readonly PoolManager _pool;
        # endregion
        
        # region SFX
        [SerializeField] private AudioClip sfxEnemyHit;
        [SerializeField] private AudioClip sfxEnemyCritical;
        [SerializeField] private AudioClip sfxEnemyKilled;
        [SerializeField] private AudioClip sfxAreaStart;
        [SerializeField] private AudioClip sfxAreaEnd;
        [SerializeField] private AudioClip sfxItem;
        # endregion

        public EffectManager(PoolManager pool) {
            _pool = pool;
            musicplayer = gameObject.GetComponent<AudioSource>();
        }

        public void SetEffectText(DamageEffect effect, string text, Color color) {
            effect.accumulatedTime = 0f;
            effect.resourceText.text = text;
            effect.resourceText.color = color;
        }

        public void SetDamageEffect(Vector3 pos, float dmg, bool isCritical = false, bool isFatal = false, bool isHeal = false) {
            var myVector = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            var effect = _pool
.Get(PoolNumber.Effect);
            effect.transform.position = pos + myVector;
            dmg = Mathf.Round(dmg * 100) * 0.01f;
            effect.transform.GetComponent<DamageEffect>().delay = 0.25f;
            if(isHeal) {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "+" + dmg, Color.green);
            }
            else if(isFatal) {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "X_X", Color.red);
            }
            else if(isCritical) {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "-" + dmg + "!", Color.yellow);
            }
            else {
                SetEffectText(effect.transform.GetComponent<DamageEffect>(), "-" + dmg, Color.white);
            }
        }

        public void PlayEnemySound(bool isCritical = false, bool isKilled = false) {
            if (isKilled)           musicplayer.PlayOneShot(sfxEnemyKilled);
            else if (isCritical)    musicplayer.PlayOneShot(sfxEnemyCritical);
            else                    musicplayer.PlayOneShot(sfxEnemyHit);
        }

        public void PlayAreaSound(bool start) {
            if (start)  musicplayer.PlayOneShot(sfxAreaStart);
            else        musicplayer.PlayOneShot(sfxAreaEnd);
        }

        public void PlayItemSound() {
            musicplayer.PlayOneShot(sfxItem);
        }
    }
}