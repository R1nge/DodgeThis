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
            var pos = new Vector3(Random.Range(-3, 5), 2, Random.Range(-5, 4));
            var inst = Instantiate(playerPrefab, pos, Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
        {
            SpawnPlayer(rpcParams.Receive.SenderClientId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}