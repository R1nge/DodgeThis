using System.Collections.Generic;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class PlayerSpawnerEyeBalls : PlayerSpawner
    {
        [SerializeField] private EyeBallsPlayerData player;
        [SerializeField] private Transform[] spawnPositions;
        private List<EyeBallsPlayerData> _currentPlayers;

        protected override void Awake()
        {
            base.Awake();
            _currentPlayers = new List<EyeBallsPlayerData>();
        }

        public EyeBallsPlayerData GetPlayerData(int index) => _currentPlayers[index];

        public int GetPlayerCount() => _currentPlayers.Count;

        protected override void SpawnPlayer(ulong ID)
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
    }
}