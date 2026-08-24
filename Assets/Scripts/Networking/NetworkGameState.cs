using Mirror;
using UnityEngine;

namespace Networking
{
    public sealed class NetworkGameState : NetworkBehaviour
    {
        public static NetworkGameState Active { get; private set; }

        [SyncVar] public int PlayerCount;
        [SyncVar] public int Wave;
        [SyncVar] public int SharedCoins;

        public override void OnStartServer()
        {
            Active = this;
            ServerSetPlayerCount(NetworkServer.connections.Count);
        }

        public override void OnStartClient()
        {
            Active = this;
        }

        [Server]
        public void ServerSetPlayerCount(int count)
        {
            PlayerCount = count;
        }

        [Server]
        public void ServerSetWave(int wave)
        {
            Wave = wave;
        }

        [Server]
        public void ServerAddCoins(int coins)
        {
            SharedCoins += coins;
        }
    }
}
