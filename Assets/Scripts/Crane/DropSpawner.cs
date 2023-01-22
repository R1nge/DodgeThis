using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace Crane
{
    public class DropSpawner : NetworkBehaviour
    {
        [SerializeField] private Vector3 spawnPos;
        [SerializeField] private GameObject[] drops;
        private GameState _gameState;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            SpawnDropServerRpc();
        }

        public IEnumerator SpawnDrop_c()
        {
            yield return new WaitForSeconds(3f);
            SpawnDropServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDropServerRpc()
        {
            var rot = Quaternion.Euler(new Vector3(-90, 0, 0));
            var inst = Instantiate(drops[Random.Range(0, drops.Length)], spawnPos, rot);
            inst.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}