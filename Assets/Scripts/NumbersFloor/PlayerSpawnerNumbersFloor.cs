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
            var pos = new Vector3(Random.Range(-3, 5), 2, Random.Range(-5, 4));
            var inst = Instantiate(playerPrefab, pos, Quaternion.identity);
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