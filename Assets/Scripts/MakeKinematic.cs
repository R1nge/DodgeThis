using Unity.Netcode;
using UnityEngine;

public class MakeKinematic : NetworkBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _rigidbody.isKinematic = true;
    }
}