﻿using Shared;
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
                character.Kill();
            }
            else
            {
                character.KillServerRpc();
            }
        }
    }
}