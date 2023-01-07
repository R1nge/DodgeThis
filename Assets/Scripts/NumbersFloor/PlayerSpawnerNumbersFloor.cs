using Lobby;
using Unity.Netcode;
using UnityEngine;

namespace NumbersFloor
{
    public class PlayerSpawnerNumbersFloor : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Skin skins;

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
            for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (ID == LobbySingleton.Instance.GetPlayersList()[i].ClientId)
                {
                    var controller = Instantiate(skins.GetController(4));
                    controller.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    controller.transform.position = pos;
                    var skin = Instantiate(skins.GetSkin(LobbySingleton.Instance.GetPlayersList()[i].SkinIndex),
                        pos, Quaternion.identity);
                    skin.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    skin.transform.parent = controller.transform;
                    skin.transform.localPosition = Vector3.zero;
                }
            }

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