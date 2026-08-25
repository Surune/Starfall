using Mirror;
using UnityEngine;

namespace Networking
{
    public sealed class ReadyMultiplayerGui : MonoBehaviour
    {
        private string address = "localhost";
        private string playerName = "Pilot";

        private void OnGUI()
        {
            var lobbyManager = NetworkLobbyManager.Instance;
            var panel = new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f - 150f, 440f, 300f);

            GUI.Box(panel, "MULTIPLAYER");
            if (NetworkClient.isConnected || NetworkServer.active)
            {
                GUI.Label(new Rect(panel.x + 24f, panel.y + 48f, 392f, 28f), "Connected. Waiting for the game to start.");
                GUI.Label(new Rect(panel.x + 24f, panel.y + 82f, 392f, 24f), "PLAYERS");
                var y = panel.y + 108f;
                foreach (var player in lobbyManager.roomSlots)
                {
                    var lobbyPlayer = player.GetComponent<NetworkLobbyPlayer>();
                    GUI.Label(new Rect(panel.x + 40f, y, 376f, 24f), lobbyPlayer.DisplayName);
                    y += 24f;
                }

                if (GUI.Button(new Rect(panel.x + 24f, panel.y + 252f, 392f, 36f), "DISCONNECT"))
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

            GUI.Label(new Rect(panel.x + 24f, panel.y + 48f, 392f, 24f), "PLAYER NAME");
            playerName = GUI.TextField(new Rect(panel.x + 24f, panel.y + 76f, 392f, 30f), playerName);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 116f, 392f, 24f), "HOST ADDRESS");
            address = GUI.TextField(new Rect(panel.x + 24f, panel.y + 144f, 392f, 30f), address);
            if (GUI.Button(new Rect(panel.x + 24f, panel.y + 196f, 188f, 36f), "HOST GAME"))
            {
                lobbyManager.StartHostGame(playerName);
            }
            if (GUI.Button(new Rect(panel.x + 228f, panel.y + 196f, 188f, 36f), "JOIN GAME"))
            {
                lobbyManager.StartClientGame(address, playerName);
            }
        }
    }
}
