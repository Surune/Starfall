using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starfall.Manager
{
    public class SFXManager : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip sfxEnemyHit;
        [SerializeField] AudioClip sfxEnemyCritical;
        [SerializeField] AudioClip sfxEnemyKilled;
        [SerializeField] AudioClip sfxAreaStart;
        [SerializeField] AudioClip sfxAreaEnd;
        [SerializeField] AudioClip sfxItem;
        [SerializeField] AudioClip sfxSynergy;
        [SerializeField] AudioClip sfxBonus;
        [SerializeField] AudioClip sfxExp;
        [SerializeField] AudioClip sfxShoot;
        [SerializeField] AudioClip sfxUpgrade;
        [SerializeField] AudioClip sfxFail;
        [SerializeField] AudioClip sfxPurchase;
        [SerializeField] AudioClip sfxSelect;
        [SerializeField] AudioClip sfxBarrier;
        [SerializeField] AudioClip sfxHit;

        public void PlayEnemySound(bool isCritical = false, bool isKilled = false)
        {
            if (isKilled)
            {
                audioSource.PlayOneShot(sfxEnemyKilled);
            }
            else if (isCritical)
            {
                audioSource.PlayOneShot(sfxEnemyCritical);
            }
            else
            {
                audioSource.PlayOneShot(sfxEnemyHit);
            }
        }

        public void PlayAreaSound(bool start)
        {
            if (start)
            {
                audioSource.PlayOneShot(sfxAreaStart);
            }
            else
            {
                audioSource.PlayOneShot(sfxAreaEnd);
            }
        }

        public void PlayItemSound()
        {
            audioSource.PlayOneShot(sfxItem);
        }

        public void PlayBonus()
        {
            audioSource.PlayOneShot(sfxBonus);
        }

        public void PlayExp()
        {
            audioSource.PlayOneShot(sfxExp);
        }

        public void PlaySynergy()
        {
            audioSource.PlayOneShot(sfxSynergy);
        }

        public void PlayShoot()
        {
            audioSource.PlayOneShot(sfxShoot);
        }

        public void PlayUpgrade()
        {
            audioSource.PlayOneShot(sfxUpgrade);
        }

        public void PlayFail()
        {
            audioSource.PlayOneShot(sfxFail);
        }

        public void PlayPurchase()
        {
            audioSource.PlayOneShot(sfxPurchase);
        }

        public void PlaySelect()
        {
            audioSource.PlayOneShot(sfxSelect);
        }

        public void PlayBarrier()
        {
            audioSource.PlayOneShot(sfxBarrier);
        }

        public void PlayHit()
        {
            audioSource.PlayOneShot(sfxHit);
        }
    }
}
