using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
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