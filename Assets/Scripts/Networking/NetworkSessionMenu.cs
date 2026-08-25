using Mirror;
using UnityEngine;

namespace Networking
{
    public sealed class NetworkSessionMenu : MonoBehaviour
    {
        private NetworkLobbyManager lobbyManager;
        private string address = "localhost";

        private void Awake()
        {
            lobbyManager = GetComponent<NetworkLobbyManager>();
        }

        private void OnGUI()
        {
            if (NetworkClient.isConnected || NetworkServer.active)
            {
                GUI.Label(new Rect(16f, 16f, 360f, 24f), "Multiplayer connected");
                if (GUI.Button(new Rect(16f, 44f, 160f, 30f), "Disconnect"))
                {
                    if (NetworkServer.active)
                    {
                        lobbyManager.StopHost();
                    }
                    else
                    {
                        lobbyManager.StopClient();
                    }
                }
                return;
            }

            GUI.Box(new Rect(16f, 16f, 260f, 150f), "Mirror Multiplayer");
            GUI.Label(new Rect(32f, 48f, 210f, 20f), "Host address");
            address = GUI.TextField(new Rect(32f, 70f, 228f, 24f), address);
            if (GUI.Button(new Rect(32f, 104f, 108f, 32f), "Host"))
            {
                lobbyManager.StartHostGame("Pilot");
            }
            if (GUI.Button(new Rect(152f, 104f, 108f, 32f), "Join"))
            {
                lobbyManager.StartClientGame(address, "Pilot");
            }
        }
    }
}
