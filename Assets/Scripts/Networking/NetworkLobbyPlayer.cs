using Mirror;

namespace Networking
{
    public sealed class NetworkLobbyPlayer : NetworkRoomPlayer
    {
        [SyncVar]
        private string displayName;

        public string DisplayName => displayName;

        public override void OnStartLocalPlayer()
        {
            CmdSetDisplayName(NetworkSessionManager.Instance.LocalPlayerName);
        }

        [Command]
        private void CmdSetDisplayName(string value)
        {
            displayName = value;
        }
    }
}
