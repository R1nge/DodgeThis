using Lobby;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace NumbersFloor
{
    public class PlayerSpawnerNumbersFloor : NetworkBehaviour
    {
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
                    var controller = Instantiate(skins.GetController(3));
                    controller.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    controller.transform.position = pos;
                    var skin = Instantiate(skins.GetSkin(LobbySingleton.Instance.GetPlayersList()[i].SkinIndex), pos + skins.GetOffset(i), Quaternion.identity);
                    skin.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    skin.transform.parent = controller.transform;
                    skin.transform.localPosition = skins.GetOffset(i);
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
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}