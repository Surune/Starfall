using System.Collections;
using Mirror;
using UnityEngine;

namespace Networking
{
    public sealed class NetworkLobbyManager : NetworkRoomManager
    {
        public static NetworkLobbyManager Instance { get; private set; }

        [SerializeField] private NetworkSessionManager sessionManager;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public void StartHostGame(string playerName)
        {
            sessionManager.SetLocalPlayerName(playerName);
            onlineScene = RoomScene;
            autoCreatePlayer = true;
            StartHost();
        }

        public void StartClientGame(string address, string playerName)
        {
            sessionManager.SetLocalPlayerName(playerName);
            networkAddress = address;
            onlineScene = RoomScene;
            autoCreatePlayer = true;
            StartClient();
        }

        public void StartSoloGame(string playerName)
        {
            StartHostGame(playerName);
            StartCoroutine(StartSoloMatch());
        }

        private IEnumerator StartSoloMatch()
        {
            while (roomSlots.Count == 0)
            {
                yield return null;
            }

            StartMatch();
        }

        public void StartMatch()
        {
            ServerChangeScene(GameplayScene);
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            autoCreatePlayer = newSceneName == RoomScene;
        }

        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient connection, GameObject roomPlayer)
        {
            var lobbyPlayer = roomPlayer.GetComponent<NetworkLobbyPlayer>();
            return sessionManager.ServerCreatePlayer(playerPrefab, lobbyPlayer.index, roomSlots.Count);
        }
    }
}
