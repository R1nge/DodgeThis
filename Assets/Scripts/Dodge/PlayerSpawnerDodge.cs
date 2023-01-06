using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class PlayerSpawnerDodge : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] positions;
        private int _lastPosition;
        private GameState _gameState;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
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

        private void SpawnPlayer(ulong ID)
        {
            if (!IsServer) return;
            var inst = Instantiate(playerPrefab, positions[_lastPosition].position, Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
            inst.transform.position = positions[_lastPosition].position;
            _lastPosition++;
        }

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