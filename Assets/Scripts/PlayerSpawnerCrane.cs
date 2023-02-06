using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnerCrane : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab, cranePrefab;
    private NetworkVariable<bool> _craneSpawned;
    private NetworkVariable<int> _playersSpawned;

    private bool IsCrane()
    {
        if (_craneSpawned.Value)
        {
            return false;
        }

        if (_playersSpawned.Value == NetworkManager.Singleton.ConnectedClients.Count)
        {
            if (!_craneSpawned.Value)
            {
                return true;
            }

            return false;
        }

        return Random.Range(0, 2) == 1;
    }

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        _craneSpawned = new NetworkVariable<bool>();
        _playersSpawned = new NetworkVariable<int>(0);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            SpawnServerRpc();
        }
    }

    private void SpawnPlayer(ulong ID)
    {
        _playersSpawned.Value++;
        if (IsCrane())
        {
            if (!_craneSpawned.Value)
            {
                var inst = Instantiate(cranePrefab, new Vector3(0, 21, 0), Quaternion.identity);
                inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                _craneSpawned.Value = true;
            }
            else
            {
                var inst = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
                inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
            }
        }
        else
        {
            var inst = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(ServerRpcReceiveParams rpcParams = default) => SpawnPlayer(rpcParams.SenderClientId);


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }
}