using Mirror;
using UnityEngine;
using Gameplay.Entities;

namespace Gameplay.Managers
{
    public class PlayerHPManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnCurrentHPChanged))] public int CurrentHP = 100;
        [SyncVar] public int MaxHP = 100;
        [SyncVar(hook = nameof(OnBarrierChanged))] public int Barrier;

        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        public override void OnStartAuthority()
        {
            GameManager.Instance.HPManager.SetHealthBar(CurrentHP, MaxHP);
        }

        public void GetDamage(int damage)
        {
            if (Barrier > 0)
            {
                Barrier--;
                return;
            }

            CurrentHP = Mathf.Max(CurrentHP - damage, 0);
        }

        public void GetBarrier(int count)
        {
            if (isServer)
            {
                Barrier += count;
                return;
            }

            CmdGetBarrier(count);
        }

        [Command]
        private void CmdGetBarrier(int count)
        {
            Barrier += count;
        }

        private void OnCurrentHPChanged(int _, int currentHP)
        {
            if (isOwned)
            {
                GameManager.Instance.HPManager.SetHealthBar(currentHP, MaxHP);
            }
        }

        private void OnBarrierChanged(int _, int barrier)
        {
            player.Barrier.SetActive(barrier > 0);
            player.BarrierCount.text = barrier.ToString();
        }
    }
}
