using Mirror;
using UnityEngine;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Wing : NetworkBehaviour
    {
        private const float MinDelay = 0.0005f;
        
        private PoolManager poolManager;
        private GameStateManager gameStateManager;
        [SyncVar(hook = nameof(OnOwnerChanged))] private Player owner;
        public static float SkillCooltimeMax = 1f;
        public static float CriticalProb = 0f;
        public static bool Freezing = false;

        public void Initialize(PoolManager poolManager, GameStateManager gameStateManager)
        {
            this.poolManager = poolManager;
            this.gameStateManager = gameStateManager;
        }

        public void SetOwner(Player player)
        {
            owner = player;
        }

        public override void OnStartServer()
        {
            Initialize(GameManager.Instance.PoolManager, GameManager.Instance.GameStateManager);
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public override void OnStartClient()
        {
            AttachToOwner();
        }

        private void OnOwnerChanged(Player _, Player value)
        {
            AttachToOwner(value);
        }

        private void AttachToOwner()
        {
            AttachToOwner(owner);
        }

        private void AttachToOwner(Player player)
        {
            transform.SetParent(player.GetComponent<PlayerManager>().WingContent, false);
        }

        public void ChangeSkillCool(float newcooltime)
        {
            if (newcooltime <= MinDelay)
            {
                newcooltime = MinDelay;
            }
            
            SkillCooltimeMax = newcooltime;
            CancelInvoke(nameof(Shoot));
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void Shoot()
        {
            if (!gameStateManager.IsPlaying)
            {
                return;
            }
            
            owner.ShootWing(transform.parent.position);
        }
    }
}
