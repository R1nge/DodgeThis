using Lobby;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class PlayerSpawnerDodge : NetworkBehaviour
    {
        [SerializeField] private Transform[] positions;
        [SerializeField] private Skin skins;
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
                    var controller = Instantiate(skins.GetController(4), positions[_lastPosition].position, Quaternion.identity);
                    controller.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    controller.transform.position = positions[_lastPosition].position;
                    var skin = Instantiate(skins.GetSkin(LobbySingleton.Instance.GetPlayersList()[i].SkinIndex), controller.transform.position + skins.GetOffset(i), Quaternion.identity);
                    skin.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    skin.transform.parent = controller.transform;
                    skin.transform.localPosition = skins.GetOffset(i);
                }
            }

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