using System.Collections.Generic;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private EyeBallsPlayerData player;
        [SerializeField] private Transform[] spawnPositions;
        private List<EyeBallsPlayerData> _currentPlayers;
        private GameState _gameState;

        public EyeBallsPlayerData GetPlayerData(int index) => _currentPlayers[index];

        public int GetPlayerCount() => _currentPlayers.Count;

        private void Awake()
        {
            _currentPlayers = new List<EyeBallsPlayerData>();
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
                    var playerInst = Instantiate(player.gameObject, Vector3.zero, Quaternion.identity);
                    playerInst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    playerInst.transform.position = spawnPositions[GetPlayerCount()].position;
                    _currentPlayers.Add(playerInst.GetComponent<EyeBallsPlayerData>());
                }
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
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}