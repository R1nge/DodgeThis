using Unity.Netcode;

namespace MapLobby
{
    public class LobbyData : NetworkBehaviour
    {
        private NetworkVariable<bool> _isReady;

        private void Awake() => _isReady = new NetworkVariable<bool>();

        [ServerRpc]
        public void ReadyServerRpc() => _isReady.Value = !_isReady.Value;

        public NetworkVariable<bool> IsReady() => _isReady;
    }
}