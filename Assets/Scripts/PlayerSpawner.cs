using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab, cranePrefab;
    private NetworkVariable<bool> _craneSpawned;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        _craneSpawned = new NetworkVariable<bool>();
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayer(NetworkManager.Singleton.LocalClientId);
    }

    private void SpawnPlayer(ulong ID)
    {
        if (!IsServer) return;
        if (Random.Range(0, 2) == 0)
        {
            var inst = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID);
        }
        else
        {
            if (!_craneSpawned.Value)
            {
                var inst = Instantiate(cranePrefab, new Vector3(8, 20, -6.15f), Quaternion.identity);
                inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID);
                _craneSpawned.Value = true;
            }
            else
            {
                var inst = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
                inst.GetComponent<NetworkObject>().SpawnWithOwnership(ID);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }
}