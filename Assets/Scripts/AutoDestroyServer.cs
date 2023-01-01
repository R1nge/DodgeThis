using Unity.Netcode;
using UnityEngine;

public class AutoDestroyServer : NetworkBehaviour
{
    [SerializeField] private float delay;

    private void Awake()
    {
        if (!IsServer) return;
        Invoke(nameof(Destroy), delay);
    }

    private void Destroy() => GetComponent<NetworkObject>().Despawn();
}