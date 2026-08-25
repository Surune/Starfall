using Mirror;
using UnityEngine;
using Gameplay.Managers;

namespace Networking
{
    public sealed class NetworkSessionManager : MonoBehaviour
    {
        private const float PlayerSpawnSpacing = 3f;
        private const float PlayerSpawnY = -2f;

        public static NetworkSessionManager Instance { get; private set; }
        public string LocalPlayerName { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void SetLocalPlayerName(string playerName)
        {
            LocalPlayerName = playerName;
        }

        [Server]
        public GameObject ServerCreatePlayer(GameObject playerPrefab, int playerIndex, int playerCount)
        {
            var firstPlayerX = -(playerCount - 1) * PlayerSpawnSpacing * 0.5f;
            var spawnPosition = new Vector3(firstPlayerX + playerIndex * PlayerSpawnSpacing, PlayerSpawnY, 0f);
            var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, GameManager.Instance.transform);
            return player;
        }
    }
}
