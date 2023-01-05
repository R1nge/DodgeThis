using Unity.Netcode;

namespace MapLobby
{
    public class LobbyData : NetworkBehaviour
    {
        private NetworkVariable<bool> _isReady;
        private LobbyDataUI _lobbyDataUI;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            _lobbyDataUI = GetComponent<LobbyDataUI>();
            _isReady = new NetworkVariable<bool>();
            _isReady.OnValueChanged += OnReadyStateChanged;
        }

        private void OnClientConnected(ulong obj)
        {
            _lobbyDataUI.UpdateReadyStateServerRpc(_isReady.Value);
        }

        private void OnReadyStateChanged(bool _, bool newValue)
        {
            _lobbyDataUI.UpdateReadyStateServerRpc(newValue);
        }

        [ServerRpc]
        public void ReadyServerRpc() => _isReady.Value = !_isReady.Value;

        public NetworkVariable<bool> IsReady() => _isReady;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}