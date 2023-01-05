using Unity.Netcode;

namespace Character
{
    public class CharacterState : NetworkBehaviour
    {
        private GameState _gameState;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            _gameState = FindObjectOfType<GameState>();
        }

        private void OnClientDisconnected(ulong obj)
        {
            if (!IsServer) return;
            if (!GetComponent<NetworkObject>().IsSpawned) return;
            _gameState.OnPlayerKilledServerRpc();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _gameState.OnCharacterSpawned();
        }

        public void Kill()
        {
            _gameState.OnPlayerKilledServerRpc();
            if (TryGetComponent(out NetworkObject networkObject))
            {
                if (!networkObject.IsSpawned) return;
                networkObject.Despawn();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}