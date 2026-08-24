using Mirror;
using UnityEngine;

namespace Networking
{
    public sealed class NetworkSessionManager : NetworkManager
    {
        private struct LobbyJoinMessage : NetworkMessage
        {
            public string PlayerName;
        }

        private struct LobbyRosterMessage : NetworkMessage
        {
            public string PlayerNames;
        }

        public static NetworkSessionManager Instance { get; private set; }

        [SerializeField] private NetworkLobbyState lobbyPrefab;
        [SerializeField] private string gameSceneName = "maingame";

        public string LocalPlayerName { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartHostGame(string playerName)
        {
            LocalPlayerName = playerName;
            onlineScene = string.Empty;
            autoCreatePlayer = false;
            StartHost();
        }

        public void StartClientGame(string address, string playerName)
        {
            LocalPlayerName = playerName;
            networkAddress = address;
            autoCreatePlayer = false;
            StartClient();
        }

        public void StartMatch()
        {
            autoCreatePlayer = true;
            ServerChangeScene(gameSceneName);
        }

        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<LobbyJoinMessage>(OnLobbyJoin);
            var lobbyState = Instantiate(lobbyPrefab);
            NetworkServer.Spawn(lobbyState.gameObject);
        }

        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<LobbyRosterMessage>(OnLobbyRoster);
        }

        public override void OnClientConnect()
        {
            NetworkClient.Ready();
            NetworkClient.Send(new LobbyJoinMessage { PlayerName = LocalPlayerName });
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            if (newSceneName == gameSceneName)
            {
                autoCreatePlayer = true;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient connection)
        {
            var index = numPlayers;
            var spawnPosition = new Vector3(-4.5f + index * 3f, -3.5f, 0f);
            var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(connection, player);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient connection)
        {
            NetworkLobbyState.Active.ServerRemovePlayer(connection.connectionId);
            SendLobbyRoster();
            base.OnServerDisconnect(connection);
        }

        private void OnLobbyJoin(NetworkConnectionToClient connection, LobbyJoinMessage message)
        {
            NetworkLobbyState.Active.ServerSetPlayerName(connection.connectionId, message.PlayerName);
            SendLobbyRoster();
        }

        private void OnLobbyRoster(LobbyRosterMessage message)
        {
            NetworkLobbyState.SetCurrentPlayerNames(message.PlayerNames);
        }

        private void SendLobbyRoster()
        {
            NetworkServer.SendToAll(new LobbyRosterMessage
            {
                PlayerNames = string.Join("\n", NetworkLobbyState.Active.PlayerNames.Values)
            });
        }
    }
}
