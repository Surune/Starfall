using System;
using UnityEngine;
using UnityEngine.UI;
using Audio;
using Core.Constants;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class DropItem : MonoBehaviour, IPoolable
    {
        private HPManager hpManager;
        private PlayerManager playerManager;
        private SoundManager sound;
        private GameStateManager gameStateManager;
        private Camera mainCamera => Camera.main;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        public Sprite[] ItemSprites;
        public ItemType Type = 0;
        [SerializeField] private Vector3 direction;
        [SerializeField] private float speed = 1f;

        public void Initialize(HPManager hpManager, PlayerManager playerManager, SoundManager sound, GameStateManager gameStateManager)
        {
            this.hpManager = hpManager;
            this.playerManager = playerManager;
            this.sound = sound;
            this.gameStateManager = gameStateManager;
        }

        private void Update()
        {
            if (gameStateManager.IsPlaying)
            {
                transform.position += direction * Time.deltaTime * speed;
            }

            var objectPosition = transform.position;
            var screenPosition = mainCamera.WorldToScreenPoint(objectPosition);
            if (screenPosition.x < 0f || screenPosition.x > Screen.width || screenPosition.y < 0f || screenPosition.y > Screen.height)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }

        public void SetType(ItemType n)
        {
            Type = n;
            spriteRenderer.sprite = ItemSprites[(int)n];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.transform.CompareTag("Player"))
            {
                return;
            }
            
            switch (Type)
            {
                case ItemType.Wing:
                    playerManager.GetWing(1);
                    break;
                case ItemType.Barrier:
                    hpManager.GetBarrier(1);
                    break;
                case ItemType.HP:
                    hpManager.ChangeHP(10);
                    break;
                case ItemType.Damage:
                    playerManager.DamageAllEnemy(playerManager.damage * playerManager.damageCoefficient);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (playerManager.repair)
            {
                hpManager.ChangeHP(10);
            }
            if (playerManager.jera)
            {
                playerManager.damage += 0.2f;
            }
            if (playerManager.dagaz)
            {
                hpManager.GetBarrier(1);
            }
            if (playerManager.reinforce)
            {
                playerManager.criticalProb += 0.1f;
            }
            sound.PlaySFX(SoundKey.Item);
            gameObject.SetActive(false);
        }
    }
}
