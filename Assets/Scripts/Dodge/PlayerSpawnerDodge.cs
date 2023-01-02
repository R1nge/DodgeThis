using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class PlayerSpawnerDodge : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] positions;
        private int _lastPosition;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        }

        public override void OnNetworkSpawn()
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }

        private void SpawnPlayer(ulong ID)
        {
            if (!IsServer) return;
            var inst = Instantiate(playerPrefab, positions[_lastPosition].position, Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID);
            inst.transform.position = positions[_lastPosition].position;
            _lastPosition++;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}