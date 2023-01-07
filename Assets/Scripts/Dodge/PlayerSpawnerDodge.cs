using Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class PlayerSpawnerDodge : NetworkBehaviour
    {
        [SerializeField] private Transform[] positions;
        [SerializeField] private PlayerSkins skins;
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
            for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (ID == LobbySingleton.Instance.GetPlayersList()[i].ClientId)
                {
                    var inst = Instantiate(skins.GetSkin(i, 0), positions[_lastPosition].position, Quaternion.identity);
                    inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    inst.transform.position = positions[_lastPosition].position;
                }
            }

            _lastPosition++;

            for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                print("DODGE");
            }
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