using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Audio;
using Core.Constants;
using Gameplay.Managers;
using Networking;
using Data.Abilities;
using Utilities;

namespace Gameplay.Entities
{
    public class Player : NetworkBehaviour
    {
        private PlayerManager playerManager;
        private PoolManager poolManager;
        private SoundManager sound;
        private GameStateManager gameStateManager;

        public GameObject Barrier;
        [SerializeField] private TMP_Text barrierCount;
        public float SkillCooltimeMax = 0.2f;
        public int BulletCount = 1;
        [SerializeField] private float speed = 5f;
        [SerializeField] private InputActionReference move;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private Rigidbody2D rigidBody;
        [HideInInspector] public bool Reloading;
        [HideInInspector] public int KillNum = 0;

        private const float MinDelay = 0.0005f;
        private Vector2 moveDir;
        private int[] networkChoiceIndexes;

        [SyncVar(hook = nameof(OnDisplayNameChanged))]
        private string displayName;

        public string DisplayName => displayName;
        public TMP_Text BarrierCount => barrierCount;

        public void Initialize(PlayerManager playerManager, PoolManager poolManager, SoundManager sound, GameStateManager gameStateManager)
        {
            this.playerManager = playerManager;
            this.poolManager = poolManager;
            this.sound = sound;
            this.gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            if (!NetworkClient.active && !NetworkServer.active)
            {
                InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
            }
        }

        private void Update()
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                if (!isOwned)
                {
                    return;
                }

                moveDir = move.action.ReadValue<Vector2>();
                CmdSetMove(moveDir);
                return;
            }

            moveDir = move.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                if (isServer)
                {
                    rigidBody.MovePosition(rigidBody.position + moveDir * (Time.fixedDeltaTime * speed));
                }

                return;
            }

            rigidBody.MovePosition(rigidBody.position + moveDir * (Time.fixedDeltaTime * speed));
        }

        public override void OnStartLocalPlayer()
        {
            GameManager.Instance.SetLocalPlayer(this);
            CmdSetDisplayName(NetworkSessionManager.Instance.LocalPlayerName);
            gameObject.name = NetworkSessionManager.Instance.LocalPlayerName;
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public override void OnStartServer()
        {
            Initialize(GetComponent<PlayerManager>(), GameManager.Instance.PoolManager, GameManager.Instance.SoundManager, GameManager.Instance.GameStateManager);
        }

        public override void OnStartClient()
        {
            transform.SetParent(GameManager.Instance.transform);
        }

        [Command]
        private void CmdSetMove(Vector2 direction)
        {
            moveDir = direction;
        }

        [Command]
        private void CmdSetDisplayName(string value)
        {
            displayName = value;
            gameObject.name = value;
        }

        [Server]
        public void ServerShowChoice(int[] abilityIndexes)
        {
            TargetShowChoice(connectionToClient, abilityIndexes);
        }

        [TargetRpc]
        private void TargetShowChoice(NetworkConnectionToClient target, int[] abilityIndexes)
        {
            networkChoiceIndexes = abilityIndexes;
            var choices = new AbilityData[abilityIndexes.Length];
            for (var i = 0; i < choices.Length; i++)
            {
                choices[i] = GameManager.Instance.AbilityManager.GetAbility(abilityIndexes[i]);
            }

            GameManager.Instance.Timer.ShowNetworkChoice(choices, SelectNetworkChoice);
        }

        private void SelectNetworkChoice(int index)
        {
            GameManager.Instance.AbilityManager.Choiced(GameManager.Instance.AbilityManager.GetAbility(networkChoiceIndexes[index]));
            CmdSelectNetworkChoice(index);
        }

        [Command]
        private void CmdSelectNetworkChoice(int index)
        {
            GameManager.Instance.Timer.ServerSelectNetworkChoice(this, index);
        }

        [Server]
        public void ServerResumeGameplay()
        {
            TargetResumeGameplay(connectionToClient);
        }

        [TargetRpc]
        private void TargetResumeGameplay(NetworkConnectionToClient target)
        {
            GameManager.Instance.GameStateManager.SetState(GameState.Gameplay);
        }

        private void OnDisplayNameChanged(string _, string value)
        {
            displayName = value;
            gameObject.name = value;
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
            if (!gameStateManager.IsPlaying || Reloading)
            {
                return;
            }

            if (NetworkClient.active || NetworkServer.active)
            {
                if (isServer)
                {
                    ServerShoot();
                }
                else if (isOwned)
                {
                    CmdShoot();
                }

                return;
            }

            SpawnBullets();
        }

        [Command]
        private void CmdShoot()
        {
            ServerShoot();
        }

        public void ShootWing(Vector3 position)
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                if (isServer)
                {
                    ServerShootWing(position);
                }
                else if (isOwned)
                {
                    CmdShootWing(position);
                }

                return;
            }

            ServerShootWing(position);
        }

        [Command]
        private void CmdShootWing(Vector3 position)
        {
            ServerShootWing(position);
        }

        [Server]
        private void ServerShootWing(Vector3 position)
        {
            poolManager.Spawn<WingBullet>(fireball =>
            {
                fireball.transform.rotation = Quaternion.identity;
                fireball.transform.position = position;
            });
        }

        [Server]
        private void ServerShoot()
        {
            SpawnBullets();
        }

        private void SpawnBullets()
        {
            sound.PlaySFX(SoundKey.Shoot);
            for (var i = 0; i < BulletCount; i++)
            {
                poolManager.Spawn<Bullet>(bullet =>
                {
                    playerManager.SetFireInfo(bullet);
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.transform.position = bulletSpawnPoint.position;
                });
            }
        }

        public void Explode(Transform center, float coeff = 1f)
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                if (isServer)
                {
                    ServerExplode(center.position, coeff);
                }
                else if (isOwned)
                {
                    CmdExplode(center.position, coeff);
                }

                return;
            }

            SpawnExplosion(center.position, coeff);
        }

        [Command]
        private void CmdExplode(Vector3 position, float coeff)
        {
            ServerExplode(position, coeff);
        }

        [Server]
        private void ServerExplode(Vector3 position, float coeff)
        {
            SpawnExplosion(position, coeff);
        }

        private void SpawnExplosion(Vector3 position, float coeff)
        {
            for (var i = -2; i <= 2; i++)
            {
                var angle = 45 * i;
                poolManager.Spawn<Bullet>(bullet =>
                {
                    bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                    bullet.Damage = playerManager.damage * playerManager.damageCoefficient * coeff;
                    playerManager.SetFireInfo(bullet);
                    bullet.transform.position = position;
                });
            }
        }

        public void Echoshot(int shotnum)
        {
            for (var i = 0; i < shotnum; i++)
            {
                Invoke(nameof(Shoot), 0.1f * i);
            }
        }
    }
}
