using Unity.Netcode;
using UnityEngine;

namespace Shared
{
    public abstract class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] protected Skin skins;
        [SerializeField] protected GameObject controller;
        private GameState _gameState;

        protected virtual void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        protected void OnGameStarted()
        {
            if (IsServer)
            {
                SpawnPlayer(NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                SpawnPlayerServerRpc();
            }
        }

        protected abstract void SpawnPlayer(ulong ID);

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
        {
            SpawnPlayer(rpcParams.Receive.SenderClientId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_gameState == null) return;
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}