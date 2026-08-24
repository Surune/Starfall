using Mirror;
using System.Collections.Generic;

namespace Networking
{
    public sealed class NetworkLobbyState : NetworkBehaviour
    {
        public static NetworkLobbyState Active { get; private set; }
        public static readonly List<string> CurrentPlayerNames = new();

        public readonly SyncDictionary<int, string> PlayerNames = new();

        public override void OnStartServer()
        {
            Active = this;
        }

        public override void OnStartClient()
        {
            Active = this;
            PlayerNames.OnChange += OnPlayerNamesChanged;
            RefreshPlayerNames();
        }

        private void OnPlayerNamesChanged(SyncIDictionary<int, string>.Operation _, int __, string ___)
        {
            RefreshPlayerNames();
        }

        private void RefreshPlayerNames()
        {
            CurrentPlayerNames.Clear();
            foreach (var playerName in PlayerNames.Values)
            {
                CurrentPlayerNames.Add(playerName);
            }
        }

        public static void SetCurrentPlayerNames(string playerNames)
        {
            CurrentPlayerNames.Clear();
            CurrentPlayerNames.AddRange(playerNames.Split('\n'));
        }

        [Server]
        public void ServerSetPlayerName(int connectionId, string playerName)
        {
            PlayerNames[connectionId] = playerName;
            RefreshPlayerNames();
        }

        [Server]
        public void ServerRemovePlayer(int connectionId)
        {
            PlayerNames.Remove(connectionId);
            RefreshPlayerNames();
        }
    }
}
