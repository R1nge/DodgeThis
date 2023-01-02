using Unity.Netcode;
using UnityEngine;

namespace NumbersFloor
{
    public class PlayerSpawnerNumbersFloor : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;

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
            //TODO spawn randomly across plane
            var inst = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}