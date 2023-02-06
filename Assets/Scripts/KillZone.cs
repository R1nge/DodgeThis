using Shared;
using Unity.Netcode;
using UnityEngine;

public class KillZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out CharacterState character))
        {
            if (!character.GetComponent<NetworkObject>().IsSpawned || character == null) return;
            if (IsServer)
            {
                character.Kill(character.OwnerClientId);
            }
            else
            {
                OnCollisionServerRpc(character.NetworkObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnCollisionServerRpc(NetworkObjectReference other)
    {
        if (other.TryGet(out NetworkObject obj))
        {
            if (obj.transform.TryGetComponent(out CharacterState character))
            {
                character.Kill(character.OwnerClientId);
            }
        }
    }
}