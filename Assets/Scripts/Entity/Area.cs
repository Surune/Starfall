using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Starfall.Manager;

namespace Starfall.Entity
{
    public class Area : MonoBehaviour
    {
        static EffectManager EffectManager => GameManager.Instance.EffectManager;
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;

        public GameObject Iconimage;
        public Image Durationimage;
        public float Duration;
        float currentTime;
        [HideInInspector] public bool Slow = false;
        [HideInInspector] public bool Damage = false;
        bool makeCritical = false;
        bool unrelenting = false;
        bool fixDamage = false;
        bool swarm = false;
        public List<GameObject> Enemies;

        void OnEnable()
        {
            SetProperty();
            EffectManager.PlayAreaSound(start : true);
            Enemies = new List<GameObject>();
            currentTime = 0f;
        }

        public void SetIcon(Sprite icon)
        {
            Iconimage.GetComponent<Image>().sprite = icon;
        }

        public void SetProperty(bool isSlow = false, bool isDamage = false, bool isCrit = false, bool isUnrelenting = false, bool isFixed = false, bool isSwarm = false)
        {
            Slow = isSlow;
            Damage = isDamage;
            makeCritical = isCrit;
            unrelenting = isUnrelenting;
            fixDamage = isFixed;
            swarm = isSwarm;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                Enemies.Add(collision.gameObject);
            }
            else if (collision.transform.CompareTag("Bullet"))
            {
                if (makeCritical && !collision.gameObject.GetComponent<Fireball>().IsCritical)
                {
                    PlayerManager.MakeCritical(collision.gameObject.GetComponent<Fireball>());
                }
                else if (unrelenting && collision.gameObject.GetComponent<Fireball>().IsCritical)
                {
                    collision.gameObject.GetComponent<Fireball>().Damage *= 2f;
                }
                else if (fixDamage)
                {
                    collision.gameObject.GetComponent<Fireball>().Damage += 1f;
                }
                else if (swarm)
                {
                    collision.gameObject.GetComponent<Fireball>().Psychosink = true;
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                Enemies.Remove(collision.gameObject);
            }
        }

        void Update()
        {
            if (GameStateManager.Instance.IsPlaying)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= Duration)
                {
                    if (Damage)
                    {
                        for (int i = Enemies.Count - 1; i >= 0; i--)
                        {
                            if(Enemies[i] != null)
                            {
                                Enemies[i].GetComponent<Enemy>().GetDamage(4, mute: false);
                            }
                        }
                    }
                    if (Slow)
                    {
                        for (int i = Enemies.Count - 1; i >= 0; i--)
                        {
                            if(Enemies[i] != null)  Enemies[i].GetComponent<Enemy>().SlowTime = 3f;
                        }
                    }
                    EffectManager.PlayAreaSound(start : false);
                    gameObject.SetActive(false);
                }
                else
                {
                    Durationimage.fillAmount = currentTime / Duration;
                }
            }
        }
    }
}
